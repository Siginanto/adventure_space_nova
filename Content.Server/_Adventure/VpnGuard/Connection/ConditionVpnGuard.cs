using Content.Server.Connection.Whitelist;
using Content.Server._Adventure.VpnGuard;

namespace Content.Server._Adventure.VpnGuard.Connection;

/// <summary>
/// Check if user uses vpn for connection.
/// </summary>
public sealed partial class ConditionVpnGuard : WhitelistCondition
{
    // [DataField]
    // public IVpnGuardManager GuardType = VpnGuardFile;
}
