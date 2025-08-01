﻿using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

public abstract class ContentFieldType(string name)
{
    public string Name { get; } = name;

    public abstract Type DotnetType { get; }

    private static IEnumerable<ContentFieldType> All =>
    [
        new ContentFieldTypeSymbol(),
        new ContentFieldTypeText(),
        new ContentFieldTypeRichText(),
        new ContentFieldTypeInteger(),
        new ContentFieldTypeNumber(),
        new ContentFieldTypeDate(),
        new ContentFieldTypeBoolean(),
        new ContentFieldTypeLink(),
        new ContentFieldTypeArray(),
        new ContentFieldTypeObject(),
    ];

    public static ContentFieldType FromName(string name)
    {
        if (TryFromName(name, out var result))
            return result!;
        
        throw new ArgumentException($"Unsupported FieldType: {name}");
    }

    public static bool TryFromName(string name, out ContentFieldType? fieldType)
    {
        fieldType = All.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return fieldType is not null;
    }

    public override string ToString() => Name;

    public abstract object? GetValue(object prop, Schema? schema);

    public abstract object? Deserialize(object? raw, Schema? schema);

    public abstract bool IsValidRawValue(object? value);
}