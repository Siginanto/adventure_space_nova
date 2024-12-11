using Content.Shared.Roles;
using Robust.Shared.Player;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Preferences;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._Adventure.Sponsors;

/// <summary>
/// Requires the player to be a sponsor.
/// </summary>
[UsedImplicitly]
[Serializable, NetSerializable]
public sealed partial class SponsorRequirement : JobRequirement
{
    [DataField]
    public int MinimumLevel = 0;

    public override bool Check(IEntityManager entManager,
        IPrototypeManager protoManager,
        HumanoidCharacterProfile? profile,
        IReadOnlyDictionary<string, TimeSpan> playTimes,
        ICommonSession? session,
        [NotNullWhen(false)] out FormattedMessage? reason)
    {
        reason = new FormattedMessage();

        if (profile is null) //the profile could be null if the player is a ghost. In this case we don't need to block the role selection for ghostrole
            return true;

        if (session is null)
            return true;

        var sponsors = IoCManager.Resolve<ISponsorsManager>();
        if (sponsors == null)
        {
            reason = FormattedMessage.FromMarkupPermissive($"Система спонсорки недоступна.");
            return false;
        }

        var sponsor = sponsors.GetSponsor(session.UserId);
        if (sponsor != null && sponsor.Level >= MinimumLevel)
            return true;

        reason = FormattedMessage.FromMarkupPermissive($"Для использования этого лодаута надо быть спонсором {MinimumLevel} уровня.");
        return false;
    }
}
