using Contentful.Core.Models.Management;
using System.Collections;

namespace Contool.Core.Infrastructure.Contentful.Models;

public class ContentLocales(IReadOnlyList<Locale> locales) : IEnumerable<string>
{
    private readonly List<string> _locales = [.. locales
        .Select(l => l.Code)
        .OrderBy(l => l)];

    public string DefaultLocale { get; } = locales
        .First(l => l.Default)
        .Code;

    public IEnumerator<string> GetEnumerator()
        => _locales.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}