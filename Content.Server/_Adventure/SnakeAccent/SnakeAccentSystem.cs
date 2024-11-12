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

        // а => ааа 
        message = Regex.Replace(
            message,
            "а+",
            _random.Pick(new List<string> { "аа", "ааа" })
        );
        // А => ААА 
        message = Regex.Replace(
            message,
            "А+",
            _random.Pick(new List<string> { "АА", "ААА" })
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
        // е => еее 
        message = Regex.Replace(
            message,
            "е+",
            _random.Pick(new List<string> { "ее", "еее" })
        );
        // Е => ЕЕЕ 
        message = Regex.Replace(
            message,
            "Е+",
            _random.Pick(new List<string> { "ЕЕ", "ЕЕЕ" })
        );
        // я => йа
        message = Regex.Replace(
            message,
            "я+",
            _random.Pick(new List<string> { "йа", "йаа" })
        );
        // Я => ЙА 
        message = Regex.Replace(
            message,
            "Я+",
            _random.Pick(new List<string> { "ЙА", "ЙАА" })
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
