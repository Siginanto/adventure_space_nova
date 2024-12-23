using Robust.Shared.Configuration;
using Robust.Shared.Log;

namespace Content.Server._Adventure;

public sealed class AdventurePresetManager
{
    [Dependency] private readonly ILogManager _log = default!;

    public void Initialize()
    {
        _log.GetSawmill("net").Level = LogLevel.Error;
    }
}
