namespace Contool.Contentful.Models;

internal class ContentFieldName
{
    public string Value { get; }
    public string Locale { get; set; }

    public ContentFieldName(string fieldId, FieldType fieldType, string locale)
    {
        Value = $"{fieldId}.{locale}" + (fieldType is ArrayFieldType ? "[]" : "");
        Locale = locale;
    }

    public ContentFieldName(string fieldName)
    {
        Value = fieldName;
        Locale = fieldName.Split('.')[1];
    }

    public override string ToString() => Value;
}
