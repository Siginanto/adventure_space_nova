using System.Text.RegularExpressions;
using Content.Server.Speech;
using Robust.Shared.Random;

namespace Content.Server._Adventure.SnakeAccent;

public sealed class SnakeAccentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SnakeAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, SnakeAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // ф => ффф 
        message = Regex.Replace(
            message,
            "ф+",
            _random.Pick(new List<string> { "фф", "ффф" })
        );
        // Ф => ФФФ 
        message = Regex.Replace(
            message,
            "Ф+",
            _random.Pick(new List<string> { "ФФ", "ФФФ" })
        );
        // ц => ссс 
        message = Regex.Replace(
            message,
            "ц+",
            _random.Pick(new List<string> { "сс", "ссс" })
        );
        // Ц => ССС 
        message = Regex.Replace(
            message,
            "Ц+",
            _random.Pick(new List<string> { "СС", "ССС" })
        );
        // г => ххх 
        message = Regex.Replace(
            message,
            "г+",
            _random.Pick(new List<string> { "хх", "ххх" })
        );
        // Г => ХХХ 
        message = Regex.Replace(
            message,
            "Г+",
            _random.Pick(new List<string> { "ХХ", "ХХХ" })
        );
        // х => ххх 
        message = Regex.Replace(
            message,
            "х+",
            _random.Pick(new List<string> { "хх", "ххх" })
        );
        // Х => ХХХ 
        message = Regex.Replace(
            message,
            "Х+",
            _random.Pick(new List<string> { "ХХ", "ХХХ" })
        );
        // й => ййй 
        message = Regex.Replace(
            message,
            "й+",
            _random.Pick(new List<string> { "йй", "ййй" })
        );
        // Й => ЙЙЙ
        message = Regex.Replace(
            message,
            "Й+",
            _random.Pick(new List<string> { "ЙЙ", "ЙЙЙ" })
        );
        // я => йа
        message = Regex.Replace(
            message,
            "я+",
            _random.Pick(new List<string> { "ййаа", "ййааа", "йййаа", "йййааа" })
        );
        // Я => ЙА 
        message = Regex.Replace(
            message,
            "Я+",
            _random.Pick(new List<string> { "ЙЙАА", "ЙЙААА", "ЙЙЙАА", "ЙЙЙААА" })
        );
        // ё => йо
        message = Regex.Replace(
            message,
            "ё+",
            _random.Pick(new List<string> { "ййоо", "ййооо", "йййоо", "йййооо" })
        );
        // Ё => ЙО 
        message = Regex.Replace(
            message,
            "Ё+",
            _random.Pick(new List<string> { "ЙЙОО", "ЙЙООО", "ЙЙЙОО", "ЙЙЙООО" })
        );
        // ю => йу
        message = Regex.Replace(
            message,
            "ю+",
            _random.Pick(new List<string> { "ййуу", "ййууу", "йййуу", "йййууу" })
        );
        // Ю => ЙУ 
        message = Regex.Replace(
            message,
            "Ю+",
            _random.Pick(new List<string> { "ЙЙУУ", "ЙЙУУУ", "ЙЙЙУУ", "ЙЙЙУУУ" })
        );
        // е => йэ
        message = Regex.Replace(
            message,
            "е+",
            _random.Pick(new List<string> { "ййээ", "ййэээ", "йййээ", "йййэээ" })
        );
        // Е => ЙЭ 
        message = Regex.Replace(
            message,
            "Е+",
            _random.Pick(new List<string> { "ЙЙЭЭ", "ЙЙЭЭЭ", "ЙЙЙЭЭ", "ЙЙЙЭЭЭ" })
        );
        // и => йи
        message = Regex.Replace(
            message,
            "и+",
            _random.Pick(new List<string> { "ййии", "ййиии", "йййии", "йййиии" })
        );
        // И => ЙИ
        message = Regex.Replace(
            message,
            "И+",
            _random.Pick(new List<string> { "ЙЙИИ", "ЙЙИИИ", "ЙЙЙИИ", "ЙЙЙИИИ" })
        );
        // c => ссс
        message = Regex.Replace(
            message,
            "с+",
            _random.Pick(new List<string>() { "сс", "ссс" })
        );
        // С => CCC
        message = Regex.Replace(
            message,
            "С+",
            _random.Pick(new List<string>() { "СС", "ССС" })
        );
        // з => ссс
        message = Regex.Replace(
            message,
            "з+",
            _random.Pick(new List<string>() { "сс", "ссс" })
        );
        // З => CCC
        message = Regex.Replace(
            message,
            "З+",
            _random.Pick(new List<string>() { "СС", "ССС" })
        );
        // ш => шшш,щщщ
        message = Regex.Replace(
            message,
            "ш+",
            _random.Pick(new List<string>() { "шш", "шшш", "щщ", "щщщ" }) 
        );
        // Ш => ШШШ,ЩЩЩ
        message = Regex.Replace(
            message,
            "Ш+",
            _random.Pick(new List<string>() { "ШШ", "ШШШ", "ЩЩ", "ЩЩЩ" }) 
        );
        // ч => щщщ,шшш
        message = Regex.Replace(
            message,
            "ч+",
            _random.Pick(new List<string>() { "щщ", "щщщ", "шш", "шшш" }) 
        );
        // Ч => ЩЩЩ,ШШШ
        message = Regex.Replace(
            message,
            "Ч+",
            _random.Pick(new List<string>() { "ЩЩ", "ЩЩЩ", "ШШ", "ШШШ" }) 
        );
        // ж => жжж 
        message = Regex.Replace(
            message,
            "ж+",
            _random.Pick(new List<string>() { "шш", "шшш", "щщ", "щщщ" })
        );
        // Ж => ЖЖЖ 
        message = Regex.Replace(
            message,
            "Ж+",
            _random.Pick(new List<string>() { "ШШ", "ШШШ", "ЩЩ", "ЩЩЩ" })
        );
        // щ => щщщ,шшш 
        message = Regex.Replace(
            message,
            "щ+",
            _random.Pick(new List<string>() { "щщ", "щщщ", "шш", "шшш" })
        );
        // Щ => ЩЩЩ,ШШШ 
        message = Regex.Replace(
            message,
            "Щ+",
            _random.Pick(new List<string>() { "ЩЩ", "ЩЩЩ", "ШШ", "ШШШ" })
        );

        args.Message = message;
    }
}
