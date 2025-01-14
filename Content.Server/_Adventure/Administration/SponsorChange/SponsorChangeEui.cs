using Content.Server.EUI;
using Content.Shared.Cloning;
using Content.Shared.Eui;
using Content.Shared.Mind;

namespace Content.Server._Adventure.Administration.SponsorChange;

public sealed class SponsorChangeEui : BaseEui
{
    public SponsorChangeEui()
    {
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        // if (msg is not SponsorChangeMessage change)
        // {
        //     Close();
        //     return;
        // }

        Close();
    }
}
