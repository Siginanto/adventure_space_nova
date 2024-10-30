// Данный код целиком и полностью является интеллектуальной собственностью Аристофана и Тёмного Воина. Все права защищены.
// Копирование и использование данного кода запрещено без разрешения правообладателя. Слава яйцам!
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Content.Shared.DeviceLinking;
using Content.Shared._Adventure.EnergyCores;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Client._Adventure.EnergyCores
{
    public sealed class ComputerEnergyCoreControlBoundUserInterface : BoundUserInterface
    {
        private EntityQuery<DeviceLinkSourceComponent> _recQuery;
        private ComputerEnergyCoreControlWindow? _window;
        [Dependency] private readonly IEntityManager _e = default!;

        public ComputerEnergyCoreControlBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }
        protected override void Open()
        {
            base.Open();

            _window = this.CreateWindow<ComputerEnergyCoreControlWindow>();
            _window.OnPowerToggleButton += value =>
            {
                if (value == true)
                    SendMessage(new EnergyCoreConsoleIsOnMessage(true));
                else
                    SendMessage(new EnergyCoreConsoleIsOnMessage(false));
            };
        }
        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            if (_window == null || state is not EnergyCoreConsoleUpdateState cast) return;
            _window.SetTimeOfLife(cast.TimeOfLife);
            _window.SetPower(cast.IsOn);
            _window.SetDamage(cast.CurDamage);

        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            _window?.Dispose();
        }
    }
}
