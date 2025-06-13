using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Contentful.Models;

namespace Contool.Contentful.Extensions;

public static class ContentTypeExtensions
{
    private static readonly ContentTypeEqualityComparer ContentTypeComparer = new();

    public static ContentType Clone(this ContentType source)
    {
        if (source is null)
            throw new InvalidOperationException("Cannot clone a null ContentType.");

        return new ContentType
        {
            SystemProperties = new SystemProperties
            {
                Id = source.SystemProperties?.Id,
                Type = source.SystemProperties?.Type,
            },
            Name = source.Name,
            Description = source.Description,
            DisplayField = source.DisplayField,
            Fields = source.Fields?.Select(f => new Field
            {
                Id = f.Id,
                Name = f.Name,
                Type = f.Type,
                Required = f.Required,
                LinkType = f.LinkType,
                Items = f.Items,
                Validations = f.Validations?
                    .Select(validator => validator.CloneSafely())
                    .Where(v => v != null)
                    .ToList(),
                Disabled = f.Disabled,
                Omitted = f.Omitted,
                Localized = f.Localized,
                DefaultValue = f.DefaultValue,
            }).ToList()
        };
    }

    public static bool IsEquivalentTo(this ContentType source, ContentType target)
    {
        return ContentTypeComparer.Equals(source, target);
    }
}
