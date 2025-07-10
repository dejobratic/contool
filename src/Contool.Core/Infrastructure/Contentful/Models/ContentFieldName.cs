namespace Contool.Core.Infrastructure.Contentful.Models;

public class ContentFieldName
{
    public string Value { get; }

    public string Locale { get; }

    public ContentFieldName(string fieldId, ContentFieldType fieldType, string locale)
    {
        Value = $"{fieldId}.{locale}" + (fieldType is ContentFieldTypeArray ? "[]" : "");
        Locale = locale;
    }

    public ContentFieldName(string fieldName)
    {
        Value = fieldName;
        Locale = fieldName.Split('.')[1].Replace("[]", "");
    }

    public override string ToString() => Value;
}
