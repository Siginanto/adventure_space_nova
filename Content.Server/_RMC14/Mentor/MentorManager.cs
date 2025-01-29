using Content.Server.GameTicking;
using Content.Server.Discord;
using Robust.Shared.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Server.Players.RateLimiting;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Mentor;
using Content.Shared.Administration;
using Content.Shared.Players.RateLimiting;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System.Text.RegularExpressions;

namespace Content.Server._RMC14.Mentor;

public sealed partial class MentorManager : IPostInjectInit
{
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly PlayerRateLimitManager _rateLimit = default!;
    [Dependency] private readonly UserDbDataManager _userDb = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;

    private string _webhookUrl = string.Empty;
    private WebhookData? _webhookData;
    private ISawmill _sawmill = default!;
    private readonly HttpClient _httpClient = new();

    private readonly Dictionary<NetUserId, DiscordRelayInteraction> _relayMessages = new();

    [GeneratedRegex(@"^https://discord\.com/api/webhooks/(\d+)/((?!.*/).*)$")]
    private static partial Regex DiscordRegex();

    private const string RateLimitKey = "MentorHelp";
    private static readonly ProtoId<JobPrototype> MentorJob = "CMSeniorEnlistedAdvisor";

    private readonly List<ICommonSession> _activeMentors = new();
    private readonly Dictionary<NetUserId, bool> _mentors = new();

    // Max embed description length is 4096, according to https://discord.com/developers/docs/resources/channel#embed-object-embed-limits
    // Keep small margin, just to be safe
    private const ushort DescriptionMax = 4000;

    // Maximum length a message can be before it is cut off
    // Should be shorter than DescriptionMax
    private const ushort MessageLengthCap = 3000;

    private async Task LoadData(ICommonSession player, CancellationToken cancel)
    {
        var userId = player.UserId;
        var isMentor = await _db.IsJobWhitelisted(player.UserId, MentorJob, cancel);

        if (!isMentor)
        {
            var dbData = await _db.GetAdminDataForAsync(userId, cancel);
            var flags = AdminFlags.None;
            if (dbData?.AdminRank?.Flags != null)
            {
                flags |= AdminFlagsHelper.NamesToFlags(dbData.AdminRank.Flags.Select(p => p.Flag));
            }

            if (dbData?.Flags != null)
            {
                flags |= AdminFlagsHelper.NamesToFlags(dbData.Flags.Select(p => p.Flag));
            }

            isMentor = flags.HasFlag(AdminFlags.MentorHelp);
        }

        _mentors[player.UserId] = isMentor;

        if (isMentor)
            _activeMentors.Add(player);
    }

    private void FinishLoad(ICommonSession player)
    {
        SendMentorStatus(player);
    }

    private void ClientDisconnected(ICommonSession player)
    {
        _mentors.Remove(player.UserId);
        _activeMentors.Remove(player);
    }

    private void OnMentorSendMessage(MentorSendMessageMsg message)
    {
        var destination = new NetUserId(message.To);
        if (!_player.TryGetSessionById(destination, out var destinationSession))
            return;

        var author = message.MsgChannel.UserId;
        if (!_player.TryGetSessionById(author, out var authorSession) ||
            !_activeMentors.Contains(authorSession))
        {
            return;
        }

        SendMentorMessage(
            destination,
            destinationSession.Name,
            author,
            authorSession.Name,
            message.Message,
            destinationSession.Channel
        );
    }

    private void OnMentorHelpMessage(MentorHelpMsg message)
    {
        if (!_player.TryGetSessionById(message.MsgChannel.UserId, out var author))
            return;

        SendMentorMessage(author.UserId, author.Name, author.UserId, author.Name, message.Message, message.MsgChannel);
    }

    private void OnDeMentor(DeMentorMsg message)
    {
        if (!_player.TryGetSessionById(message.MsgChannel.UserId, out var session) ||
            !_activeMentors.Contains(session))
        {
            return;
        }

        _activeMentors.Remove(session);
        SendMentorStatus(session);
    }

    private void OnReMentor(ReMentorMsg message)
    {
        if (!_player.TryGetSessionById(message.MsgChannel.UserId, out var session) ||
            !_mentors.TryGetValue(session.UserId, out var mentor) ||
            !mentor)
        {
            return;
        }

        _activeMentors.Add(session);
        SendMentorStatus(session);
    }

    private void SendMentorStatus(ICommonSession player)
    {
        var isMentor = _activeMentors.Contains(player);
        var canReMentor = _mentors.TryGetValue(player.UserId, out var mentor) && mentor;
        var msg = new MentorStatusMsg()
        {
            IsMentor = isMentor,
            CanReMentor = canReMentor,
        };

        _net.ServerSendMessage(msg, player.Channel);
    }

    private void SendMentorMessage(NetUserId destination, string destinationName, NetUserId author, string authorName, string message, INetChannel destinationChannel)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        var recipients = new HashSet<INetChannel> { destinationChannel };
        var isMentor = false;
        foreach (var active in _activeMentors)
        {
            if (active.UserId == author)
                isMentor = true;

            recipients.Add(active.Channel);
        }

        var mentorMsg = new MentorMessage(
            destination,
            destinationName,
            author,
            authorName,
            message,
            DateTime.Now,
            isMentor
        );
        var messages = new List<MentorMessage> { mentorMsg };
        var receive = new MentorMessagesReceivedMsg { Messages = messages };
        foreach (var recipient in recipients)
        {
            try
            {
                _net.ServerSendMessage(receive, recipient);
            }
            catch (Exception e)
            {
                _sawmill.Error($"Error sending mentor help message:\n{e}");
            }
        }
    }

    private async void ProcessQueue(NetUserId userId, Queue<DiscordRelayedData> messages)
    {
        // Whether an embed already exists for this player
        var exists = _relayMessages.TryGetValue(userId, out var existingEmbed);

        // Whether the message will become too long after adding these new messages
        var tooLong = exists && messages.Sum(msg => Math.Min(msg.Message.Length, MessageLengthCap) + "\n".Length)
            + existingEmbed?.Description.Length > DescriptionMax;

        // If there is no existing embed, or it is getting too long, we create a new embed
        if (!exists || tooLong)
        {
            var lookup = await _playerLocator.LookupIdAsync(userId);

            if (lookup == null)
            {
                _sawmill.Log(LogLevel.Error,
                             $"Unable to find player for NetUserId {userId} when sending discord webhook.");
                _relayMessages.Remove(userId);
                return;
            }

            var linkToPrevious = string.Empty;

            // If we have all the data required, we can link to the embed of the previous round or embed that was too long
            if (_webhookData is { GuildId: { } guildId, ChannelId: { } channelId })
            {
                if (tooLong && existingEmbed?.Id != null)
                {
                    linkToPrevious =
                        $"**[Go to previous embed of this round](https://discord.com/channels/{guildId}/{channelId}/{existingEmbed.Id})**\n";
                }
                else if (_oldMessageIds.TryGetValue(userId, out var id) && !string.IsNullOrEmpty(id))
                {
                    linkToPrevious =
                        $"**[Go to last round's conversation with this player](https://discord.com/channels/{guildId}/{channelId}/{id})**\n";
                }
            }

            var characterName = _minds.GetCharacterName(userId);
            existingEmbed = new DiscordRelayInteraction()
            {
                Id = null,
                CharacterName = characterName,
                Description = linkToPrevious,
                Username = lookup.Username,
                LastRunLevel = _gameTicker.RunLevel,
            };

            _relayMessages[userId] = existingEmbed;
        }

        // Previous message was in another RunLevel, so show that in the embed
        if (existingEmbed!.LastRunLevel != _gameTicker.RunLevel)
        {
            existingEmbed.Description += _gameTicker.RunLevel switch
            {
                GameRunLevel.PreRoundLobby => "\n\n:arrow_forward: _**Pre-round lobby started**_\n",
                    GameRunLevel.InRound => "\n\n:arrow_forward: _**Round started**_\n",
                    GameRunLevel.PostRound => "\n\n:stop_button: _**Post-round started**_\n",
                    _ => throw new ArgumentOutOfRangeException(nameof(_gameTicker.RunLevel),
                                                               $"{_gameTicker.RunLevel} was not matched."),
                    };

            existingEmbed.LastRunLevel = _gameTicker.RunLevel;
        }

        // If last message of the new batch is SOS then relay it to on-call.
        // ... as long as it hasn't been relayed already.
        var discordMention = messages.Last();
        var onCallRelay = !discordMention.Receivers && !existingEmbed.OnCall;

        // Add available messages to the embed description
        while (messages.TryDequeue(out var message))
        {
            string text;

            // In case someone thinks they're funny
            if (message.Message.Length > MessageLengthCap)
                text = message.Message[..(MessageLengthCap - TooLongText.Length)] + TooLongText;
            else
                text = message.Message;

            existingEmbed.Description += $"\n{text}";
        }

        var payload = GeneratePayload(existingEmbed.Description,
                                      existingEmbed.Username,
                                      existingEmbed.CharacterName);

        // If there is no existing embed, create a new one
        // Otherwise patch (edit) it
        if (existingEmbed.Id == null)
        {
            var request = await _httpClient.PostAsync($"{_webhookUrl}?wait=true",
                                                      new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            var content = await request.Content.ReadAsStringAsync();
            if (!request.IsSuccessStatusCode)
            {
                _sawmill.Log(LogLevel.Error,
                             $"Discord returned bad status code when posting message (perhaps the message is too long?): {request.StatusCode}\nResponse: {content}");
                _relayMessages.Remove(userId);
                return;
            }

            var id = JsonNode.Parse(content)?["id"];
            if (id == null)
            {
                _sawmill.Log(LogLevel.Error,
                             $"Could not find id in json-content returned from discord webhook: {content}");
                _relayMessages.Remove(userId);
                return;
            }

            existingEmbed.Id = id.ToString();
        }
        else
        {
            var request = await _httpClient.PatchAsync($"{_webhookUrl}/messages/{existingEmbed.Id}",
                                                       new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            if (!request.IsSuccessStatusCode)
            {
                var content = await request.Content.ReadAsStringAsync();
                _sawmill.Log(LogLevel.Error,
                             $"Discord returned bad status code when patching message (perhaps the message is too long?): {request.StatusCode}\nResponse: {content}");
                _relayMessages.Remove(userId);
                return;
            }
        }

        _relayMessages[userId] = existingEmbed;

        // Actually do the on call relay last, we just need to grab it before we dequeue every message above.
        if (onCallRelay &&
            _onCallData != null)
        {
            existingEmbed.OnCall = true;
        }
        else
        {
            existingEmbed.OnCall = false;
        }

        _processingChannels.Remove(userId);
    }

    private async void OnWebhookChanged(string url)
    {
        _webhookUrl = url;
        if (url == string.Empty)
            return;

        // Basic sanity check and capturing webhook ID and token
        var match = DiscordRegex().Match(url);

        if (!match.Success)
        {
            // TODO: Ideally, CVar validation during setting should be better integrated
            Log.Warning("Webhook URL does not appear to be valid. Using anyways...");
            return;
        }

        if (match.Groups.Count <= 2)
        {
            Log.Error("Could not get webhook ID or token.");
            return;
        }

        var webhookId = match.Groups[1].Value;
        var webhookToken = match.Groups[2].Value;

        // Fire and forget
        _webhookData = await GetWebhookData(webhookId, webhookToken);
    }

    private async Task<WebhookData?> GetWebhookData(string id, string token)
    {
        var response = await _httpClient.GetAsync($"https://discord.com/api/v10/webhooks/{id}/{token}");

        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _sawmill.Log(LogLevel.Error,
                         $"Discord returned bad status code when trying to get webhook data (perhaps the webhook URL is invalid?): {response.StatusCode}\nResponse: {content}");
            return null;
        }

        return JsonSerializer.Deserialize<WebhookData>(content);
    }

    private static DiscordRelayedData GenerateMentorMessage(MentorMessageParams parameters)
    {
        var stringbuilder = new StringBuilder();

        if (parameters.NoReceivers)
            stringbuilder.Append(":sos:");
        else
            stringbuilder.Append(":inbox_tray:");

        if (parameters.RoundTime != string.Empty && parameters.RoundState == GameRunLevel.InRound)
            stringbuilder.Append($" **{parameters.RoundTime}**");
        stringbuilder.Append($" **{parameters.Username}** ");
        stringbuilder.Append(parameters.Message);

        return new DiscordRelayedData()
        {
            Receivers = !parameters.NoReceivers,
            Message = stringbuilder.ToString(),
        };
    }

    void IPostInjectInit.PostInject()
    {
        _net.RegisterNetMessage<MentorStatusMsg>();
        _net.RegisterNetMessage<MentorSendMessageMsg>(OnMentorSendMessage);
        _net.RegisterNetMessage<MentorHelpMsg>(OnMentorHelpMessage);
        _net.RegisterNetMessage<MentorMessagesReceivedMsg>();
        _net.RegisterNetMessage<DeMentorMsg>(OnDeMentor);
        _net.RegisterNetMessage<ReMentorMsg>(OnReMentor);
        _userDb.AddOnLoadPlayer(LoadData);
        _userDb.AddOnFinishLoad(FinishLoad);
        _userDb.AddOnPlayerDisconnect(ClientDisconnected);
        _cfg.OnValueChanged(CCVars.DiscordMentorWebhook, OnWebhookChanged, true);
        _rateLimit.Register(
            RateLimitKey,
            new RateLimitRegistration(
                RMCCVars.RMCMentorHelpRateLimitPeriod,
                RMCCVars.RMCMentorHelpRateLimitCount,
                _ => { }
            )
        );
        _sawmill = _log.GetSawmill("mentor_help");

        var defaultParams = new MentorMessageParams(
            string.Empty,
            string.Empty,
            true,
            _gameTicker.RoundDuration().ToString("hh\\:mm\\:ss"),
            _gameTicker.RunLevel,
            playedSound: false
        );
        _maxAdditionalChars = GenerateMentorMessage(defaultParams).Message.Length;
    }

    private record struct DiscordRelayedData
    {
        /// <summary>
        /// Was anyone online to receive it.
        /// </summary>
        public bool Receivers;

        /// <summary>
        /// What's the payload to send to discord.
        /// </summary>
        public string Message;
    }

    private sealed class DiscordRelayInteraction
    {
        public string? Id;

        public string Username = String.Empty;

        public string? CharacterName;

        /// <summary>
        /// Contents for the discord message.
        /// </summary>
        public string Description = string.Empty;

        /// <summary>
        /// Run level of the last interaction. If different we'll link to the last Id.
        /// </summary>
        public GameRunLevel LastRunLevel;

        /// <summary>
        /// Did we relay this interaction to OnCall previously.
        /// </summary>
        public bool OnCall;
    }


    public sealed class MentorMessageParams
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public string RoundTime { get; set; }
        public GameRunLevel RoundState { get; set; }
        public bool NoReceivers { get; set; }

        public MentorMessageParams(
            string username,
            string message,
            string roundTime,
            GameRunLevel roundState,
            bool noReceivers = false)
        {
            Username = username;
            Message = message;
            IsAdmin = isAdmin;
            RoundTime = roundTime;
            RoundState = roundState;
            NoReceivers = noReceivers;
        }
    }
}
