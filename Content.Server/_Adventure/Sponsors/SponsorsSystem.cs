using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Content.Shared._Adventure.Sponsors;

namespace Content.Server._Adventure.Sponsors;

public sealed class SponsorsSystem : EntitySystem
{
    [Dependency] private readonly SponsorsManager _sponsors = default!;
    private ISawmill _sawmill = default!;

    public override void Initialize()
    {
        _sawmill = Logger.GetSawmill("sponsors");
        _sponsors.OnSponsorConnected += OnConnected;
    }

    private void OnConnected(INetChannel chan, ProtoId<SponsorTierPrototype> proto)
    {
        _sawmill.Debug($"Sponsor connected {chan.UserName}, sending sponsor tier \"{proto}\"");
        RaiseNetworkEvent(new SponsorUpdateInfo(proto), chan);
    }
}
