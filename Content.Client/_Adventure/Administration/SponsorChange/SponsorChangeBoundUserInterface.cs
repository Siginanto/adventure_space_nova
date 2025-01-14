using Content.Shared.Cargo;
using Content.Client.Cargo.UI;
using Content.Shared.Cargo.BUI;
using Content.Shared.Cargo.Events;
using Content.Shared.Cargo.Prototypes;
using Content.Shared.IdentityManagement;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Utility;
using Robust.Shared.Prototypes;
using static Robust.Client.UserInterface.Controls.BaseButton;

namespace Content.Client._Adventure.Administration.SponsorChange;

public sealed class SponsorChangeBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private SponsorChangeWindow? _window;

    public SponsorChangeBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();
        _window = new SponsorChangeWindow();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;
        _window?.Dispose();
    }
}
