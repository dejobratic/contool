using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Core.Tests.Unit.Helpers;

public class ContentTypeBuilder
{
    private string _id = "testContentType";
    private string _name = "Test Content Type";
    private string _description = "A test content type";
    private List<Field> _fields = [];

    public ContentTypeBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public ContentTypeBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ContentTypeBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ContentTypeBuilder WithFields(params Field[] fields)
    {
        _fields = fields.ToList();
        return this;
    }

    public ContentTypeBuilder WithField(string id, string name, string type, bool required = false)
    {
        var field = new Field
        {
            Id = id,
            Name = name,
            Type = type,
            Required = required
        };
        _fields.Add(field);
        return this;
    }

    public ContentType Build()
    {
        return new ContentType
        {
            SystemProperties = new SystemProperties
            {
                Id = _id,
                Type = "ContentType"
            },
            Name = _name,
            Description = _description,
            Fields = _fields
        };
    }

    public static ContentTypeBuilder Create() => new();

    public static ContentType CreateDefault() => new ContentTypeBuilder().Build();

    public static ContentType CreateBlogPost() =>
        new ContentTypeBuilder()
            .WithId("blogPost")
            .WithName("Blog Post")
            .WithDescription("A blog post content type")
            .WithField("title", "Title", "Symbol", required: true)
            .WithField("body", "Body", "RichText", required: true)
            .WithField("author", "Author", "Symbol", required: false)
            .WithField("publishDate", "Publish Date", "Date", required: false)
            .Build();

    public static ContentType CreateProduct() =>
        new ContentTypeBuilder()
            .WithId("product")
            .WithName("Product")
            .WithDescription("A product content type")
            .WithField("name", "Name", "Symbol", required: true)
            .WithField("price", "Price", "Number", required: true)
            .WithField("description", "Description", "Text", required: false)
            .WithField("inStock", "In Stock", "Boolean", required: false)
            .Build();

    public static ContentType CreateMinimal() =>
        new ContentTypeBuilder()
            .WithId("minimal")
            .WithName("Minimal")
            .WithDescription("A minimal content type")
            .WithField("title", "Title", "Symbol", required: true)
            .Build();

    public static ContentType CreateWithBooleanField() =>
        new ContentTypeBuilder()
            .WithId("booleanTest")
            .WithName("Boolean Test")
            .WithDescription("A content type with boolean field")
            .WithField("title", "Title", "Symbol", required: true)
            .WithField("isPublished", "Is Published", "Boolean", required: false)
            .Build();

    public static ContentType CreateWithNumericField() =>
        new ContentTypeBuilder()
            .WithId("numericTest")
            .WithName("Numeric Test")
            .WithDescription("A content type with numeric field")
            .WithField("title", "Title", "Symbol", required: true)
            .WithField("viewCount", "View Count", "Integer", required: false)
            .Build();
}