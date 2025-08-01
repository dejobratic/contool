﻿using Contool.Core.Infrastructure.Extensions;

namespace Contool.Core.Infrastructure.Contentful.Extensions;

public static class LinkExtensions
{
    public static object? ToLink(this object value, string? linkType)
    {
        if (value is string linkValue && string.IsNullOrEmpty(linkValue))
        {
            return null;
        }

        var obj = new
        {
            sys = new
            {
                type = "Link",
                linkType = linkType ?? "Entry",
                id = value,
            }
        };

        return obj.SerializeToJsonObject();
    }
}
