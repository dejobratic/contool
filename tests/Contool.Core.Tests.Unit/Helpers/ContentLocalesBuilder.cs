using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Models;

namespace Contool.Core.Tests.Unit.Helpers;

public class ContentLocalesBuilder
{
    private string _defaultLocale = "en-US";
    private List<string> _locales = ["en-US"];

    public ContentLocalesBuilder WithDefaultLocale(string defaultLocale)
    {
        _defaultLocale = defaultLocale;
        return this;
    }

    public ContentLocalesBuilder WithLocales(params string[] locales)
    {
        _locales = locales.ToList();
        return this;
    }

    public ContentLocalesBuilder AddLocale(string locale)
    {
        if (!_locales.Contains(locale))
        {
            _locales.Add(locale);
        }
        return this;
    }

    public ContentLocales Build()
    {
        var locales = new List<Locale> { new Locale { Code = _defaultLocale, Default = true} };
        locales.AddRange(_locales.Where(locale => locale != _defaultLocale).Select(locale => new Locale { Code = locale, Default = false }));
        
        return new ContentLocales(locales);
    }

    public static ContentLocalesBuilder Create()
        => new();

    public static ContentLocales CreateDefault()
        => new ContentLocalesBuilder().Build();

    public static ContentLocales CreateMultilingual() =>
        new ContentLocalesBuilder()
            .WithDefaultLocale("en-US")
            .WithLocales("en-US", "es", "fr", "de")
            .Build();

    public static ContentLocales CreateSpanishDefault() =>
        new ContentLocalesBuilder()
            .WithDefaultLocale("es")
            .WithLocales("es", "en-US")
            .Build();
}