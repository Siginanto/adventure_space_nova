using Content.Shared._Adventure.EnergyCores;

namespace Content.Server._Adventure.EnergyCores
{
    [RegisterComponent]
    public sealed partial class EnergyCoreKeyComponent : Component
    {
        [DataField][ViewVariables(VVAccess.ReadOnly)]
        public EnergyCoreKeyState Key = EnergyCoreKeyState.None;
    }
}
