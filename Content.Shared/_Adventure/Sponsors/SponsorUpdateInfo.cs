using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Sponsors;

[Serializable, NetSerializable]
public sealed class SponsorUpdateInfo : EntityEventArgs
{
    public ProtoId<SponsorTierPrototype>? PrototypeId { get; }

    public SponsorUpdateInfo(ProtoId<SponsorTierPrototype>? prototypeId)
    {
        PrototypeId = prototypeId;
    }
}
