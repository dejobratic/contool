using Contentful.Core.Models.Management;

namespace Contool.Contentful.Models;

public class ContentLocales(IEnumerable<Locale> locales)
{
    public string DefaultLocale { get; } = locales
        .First(l => l.Default)
        .Code;

    public string[] Locales { get; } = [.. locales
        .Where(l => !l.Default)
        .Select(l => l.Code)
        .OrderBy(l => l)];

    public string[] GetAllLocales() 
        => [DefaultLocale, .. Locales];
}