using Content.Shared._Adventure.Sponsors;

namespace Content.Server.Chat.Managers;

internal sealed partial class ChatManager : IChatManager
{
    [Dependency] private readonly ISponsorsManager _sponsors = default!;
}
