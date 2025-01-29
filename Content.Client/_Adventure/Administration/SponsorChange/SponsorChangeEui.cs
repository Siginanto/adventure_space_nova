using System.Linq;
using Content.Client.Eui;
using Content.Shared.Cloning;
using Content.Shared.Eui;
using Content.Shared._Adventure.Sponsors;
using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;

namespace Content.Client._Adventure.Administration.SponsorChange;

[UsedImplicitly]
public sealed class SponsorChangeEui : BaseEui
{
    [Dependency] private readonly IPrototypeManager _proto = default!;

    private readonly SponsorChangeWindow _window;

    public SponsorChangeEui()
    {
        IoCManager.InjectDependencies(this);
        var protos = _proto.EnumeratePrototypes<SponsorTierPrototype>().ToList();
        _window = new SponsorChangeWindow(protos);
        _window.OnUsernameEntered += (username) => SendMessage(GetPlayerSponsorInfoRequest(username));
        _window.OnTierSelected += (tier) => SetSponsorTierRequest(_window.GetUsername(), tier);
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
    }

    public override void Opened()
    {
        IoCManager.Resolve<IClyde>().RequestWindowAttention();
        _window.OpenCentered();
    }

    public override void Closed()
    {
        _window.Close();
    }

    private void OnUsernameEntered(string username)
    {
        // var player = _db.GetPlayerRecordByUserName(username);
        // if (player == null)
        // {
        //     _window.SetMenuEnabled(false);
        //     return;
        // }
        // _window.SetMenuEnabled(true);
        // _window.SelectSponsorTier(player.SponsorTier ?? null);
    }

    private void OnTierSelected(string? tier)
    {
        // var player = _db.GetPlayerRecordByUserName(_window.GetUsername());
        // if (player == null)
        // {
        //     // По-идее такого быть не должно, но на всякий случай.
        //     _window.SetMenuEnabled(false);
        //     return;
        // }
        // _db.SetPlayerRecordSponsor(player.UserId, tier);
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is not SponsorChangeEuiState s)
            return;

        if (_proto.Index<>(s.Tier, out var proto))

        _window.SetPlayerName(s.Username);
        _window.SetMenuEnabled(s.IsValid);
        if (s.IsValid)
            _window.SelectSponsorTier(proto ?? null);
    }
}
