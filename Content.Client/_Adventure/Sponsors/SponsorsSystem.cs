using Robust.Shared.Prototypes;
using Content.Shared._Adventure.Sponsors;

namespace Content.Client._Adventure.Sponsors;

public sealed class SponsorsSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly SponsorsManager _sponsors = default!;

    private ISawmill _sawmill = default!;

    public override void Initialize()
    {
        _sawmill = Logger.GetSawmill("sponsors");
        SubscribeNetworkEvent<SponsorUpdateInfo>(OnSponsorUpdate);
    }

    private void OnSponsorUpdate(SponsorUpdateInfo ev)
    {
        _sawmill.Debug($"Get sponsor info: \"{ev.PrototypeId}\"");
        if (!_proto.TryIndex(ev.PrototypeId, out var sponsor))
        {
            _sawmill.Error($"Unknown sponsor prototype: \"{ev.PrototypeId}\"");
            return;
        }
        _sponsors.SetSponsor(sponsor);
    }
}
