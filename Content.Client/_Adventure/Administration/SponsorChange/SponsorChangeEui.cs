using Content.Client.Eui;
using Content.Shared.Cloning;
using JetBrains.Annotations;
using Robust.Client.Graphics;

namespace Content.Client._Adventure.Administration.SponsorChange;
[UsedImplicitly]
public sealed class SponsorChangeEui : BaseEui
{
    [Dependency] private readonly IServerDbManager _db = default!;

    private readonly SponsorChangeWindow _window;

    public SponsorChangeEui()
    {
        _window = new SponsorChangeWindow();
        _window.OnUsernameEntered += OnUsernameEntered;
        _window.OnTierSelected += OnTierSelected;
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
        var player = _db.GetPlayerRecordByUserName(username);
        if (player == null)
        {
            _window.SetMenuEnabled(false);
            return;
        }
        _window.SetMenuEnabled(true);
        _window.SelectSponsorTier(player.SponsorTier ?? null);
    }

    private void OnTierSelected(string? tier)
    {
        var player = _db.GetPlayerRecordByUserName(_window.GetUsername());
        if (player == null)
        {
            // По-идее такого быть не должно, но на всякий случай.
            _window.SetMenuEnabled(false);
            return;
        }
        player.SponsorTier = tier;
        _db.UpdatePlayerRecord(player.)
    }
}
