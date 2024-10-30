// Данный код целиком и полностью является интеллектуальной собственностью Аристофана и Тёмного Воина. Все права защищены.
// Копирование и использование данного кода запрещено без разрешения правообладателя. Слава яйцам!

namespace Content.Client._Adventure.EnergyCores;

[RegisterComponent]
public sealed partial class EnergyCoreVisualsComponent : Component
{
    [DataField(required: true)]
    public string OnState = default!;

    [DataField(required: true)]
    public string OffState = default!;


}

