using Contentful.Core.Models.Management;

namespace Contool.Core.Tests.Unit.Helpers;

public class LocaleBuilder
{
    private string _code = "en-US";
    private string _name = "English (United States)";
    private bool _default = true;

    public LocaleBuilder WithCode(string code)
    {
        _code = code;
        return this;
    }

    public LocaleBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public LocaleBuilder WithDefault(bool isDefault)
    {
        _default = isDefault;
        return this;
    }

    public Locale Build()
    {
        return new Locale
        {
            Code = _code,
            Name = _name,
            Default = _default
        };
    }

    public static LocaleBuilder Create() => new();

    public static Locale CreateDefault() => new LocaleBuilder().Build();

    public static Locale CreateEnglish() =>
        new LocaleBuilder()
            .WithCode("en-US")
            .WithName("English (United States)")
            .WithDefault(true)
            .Build();

    public static Locale CreateFrench() =>
        new LocaleBuilder()
            .WithCode("fr-FR")
            .WithName("French (France)")
            .WithDefault(false)
            .Build();

    public static Locale CreateSpanish() =>
        new LocaleBuilder()
            .WithCode("es-ES")
            .WithName("Spanish (Spain)")
            .WithDefault(false)
            .Build();

    public static List<Locale> CreateMultiple() =>
    [
        CreateEnglish(),
        CreateFrench(),
        CreateSpanish()
    ];
}