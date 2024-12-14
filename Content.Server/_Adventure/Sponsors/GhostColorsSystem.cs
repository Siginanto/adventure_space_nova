using Content.Shared.Ghost;
using Content.Shared._Adventure.Sponsors;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._Adventure.Sponsors;

public sealed class GhostColorsSystem : EntitySystem
{
    [Dependency] private readonly ISponsorsManager _sponsors = default!;
    private ISawmill _sawmill = default!;

    public override void Initialize()
    {
        _sawmill = Logger.GetSawmill("ghost_colors");
        SubscribeLocalEvent<GhostComponent, PlayerAttachedEvent>(OnPlayerAttached);
    }

    private void OnPlayerAttached(Entity<GhostComponent> ent, ref PlayerAttachedEvent args)
    {
        var sponsor = _sponsors.GetSponsor(args.Player.UserId);
        if (sponsor != null && sponsor.GhostColor is Color color)
            ent.Comp.color = color;
        Dirty(ent);
    }
}
