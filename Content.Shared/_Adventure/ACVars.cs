using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._Adventure.ACVar;

// ReSharper disable once InconsistentNaming
[CVarDefs]
public sealed class ACVars : CVars
{
    // c4llv07e vpn guard begin
    public static readonly CVarDef<string> VpnGuardApiUrl =
        CVarDef.Create("vpnguard.api_url", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> VpnGuardApiUserId =
        CVarDef.Create("vpnguard.api_userid", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> VpnGuardApiKey =
        CVarDef.Create("vpnguard.api_key", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> VpnGuardFilePath =
        CVarDef.Create("vpnguard.file_path", "", CVar.SERVERONLY);
    // c4llv07e vpn guard end
}
