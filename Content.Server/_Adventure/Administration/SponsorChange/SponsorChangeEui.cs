using Content.Server.EUI;
using Content.Shared.Cloning;
using Content.Shared.Eui;
using Content.Shared.Mind;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Administration.SponsorChange;

public sealed class SponsorChangeEui : BaseEui
{
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly IAdminManager _admins = default!;

    public SponsorChangeEui()
    {
        IoCManager.InjectDependencies(this);
        _sawmill = _log.GetSawmill("sponsor.changes");
    }

    public override EuiStateBase GetNewState()
    {
        return new SponsorChangeEuiState(username);
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);
        switch (msg)
        {
            case SponsorChangeEuiStateMsg.SetSponsorTierRequest r:
                SetSponsor(r.Username, r.Tier);
                break;
            case SponsorChangeEuiStateMsg.GetPlayerSponsorInfoRequest r:
                GetSponsorInfo(r.Username);
                break;
        }
    }

    public void SetSponsor(string username, ProtoId<SponsorTierPrototype>? tier)
    {
        StateDirty();
    }

    public void GetSponsorInfo(string username)
    {
    }
}
