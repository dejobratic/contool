using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Utils;
using Newtonsoft.Json.Linq;

namespace Contool.Core.Tests.Unit.Helpers;

public class EntryBuilder
{
    private string? _id;
    private string? _contentTypeId;
    private Dictionary<string, object> _fields = [];
    private DateTime? _archivedAt;
    private int? _version;

    public EntryBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public EntryBuilder WithContentTypeId(string contentTypeId)
    {
        _contentTypeId = contentTypeId;
        return this;
    }

    public EntryBuilder WithField(string fieldId, object value)
    {
        _fields[fieldId] = value;
        return this;
    }

    public EntryBuilder WithFields(Dictionary<string, object> fields)
    {
        _fields = new Dictionary<string, object>(fields);
        return this;
    }

    public EntryBuilder WithLocalizedField(string fieldId, string locale, object value)
    {
        var localizedFieldId = $"{fieldId}.{locale}";
        _fields[localizedFieldId] = value;
        return this;
    }

    public EntryBuilder WithArchivedAt(DateTime archivedAt)
    {
        _archivedAt = archivedAt;
        return this;
    }

    public EntryBuilder AsArchived()
    {
        _archivedAt = DateTime.UtcNow;
        return this;
    }

    public EntryBuilder WithVersion(int version)
    {
        _version = version;
        return this;
    }

    public Entry<dynamic> Build()
    {
        var entry = new Entry<dynamic>
        {
            SystemProperties = new SystemProperties
            {
                Id = _id,
                Type = "Entry",
                ArchivedAt = _archivedAt,
                Version = _version
            },
            Fields = JObject.FromObject(_fields)
        };

        if (!string.IsNullOrEmpty(_contentTypeId))
        {
            entry.SystemProperties.ContentType = new ContentType
            {
                SystemProperties = new SystemProperties
                {
                    Id = _contentTypeId,
                    Type = "ContentType"
                }
            };
        }

        return entry;
    }

    public static EntryBuilder Create() => new();

    public static Entry<dynamic> CreateDefault() => new EntryBuilder().Build();

    public static Entry<dynamic> CreateBlogPost(string? id = null, string contentTypeId = "blogPost") =>
        new EntryBuilder()
            .WithId(id ?? ContentfulIdGenerator.NewId())
            .WithContentTypeId(contentTypeId)
            .WithField("title", "Test Blog Post")
            .WithField("body", "This is a test blog post content")
            .WithField("author", "Test Author")
            .Build();

    public static Entry<dynamic> CreateProduct(string? id = null, string contentTypeId = "product") =>
        new EntryBuilder()
            .WithId(id ?? ContentfulIdGenerator.NewId())
            .WithContentTypeId(contentTypeId)
            .WithField("name", "Test Product")
            .WithField("price", 29.99)
            .WithField("description", "A test product")
            .WithField("inStock", true)
            .Build();

    public static Entry<dynamic> CreateMinimal(string? id = null, string contentTypeId = "minimal") =>
        new EntryBuilder()
            .WithId(id ?? ContentfulIdGenerator.NewId())
            .WithContentTypeId(contentTypeId)
            .WithField("title", "Test Title")
            .Build();

    public static Entry<dynamic> CreateWithMissingRequiredFields(string? id = null, string contentTypeId = "blogPost") =>
        new EntryBuilder()
            .WithId(id ?? ContentfulIdGenerator.NewId())
            .WithContentTypeId(contentTypeId)
            .WithField("author", "Test Author")
            .Build();

    public static Entry<dynamic> CreateWithInvalidFields(string? id = null, string contentTypeId = "invalidFields") =>
        new EntryBuilder()
            .WithId(id ?? ContentfulIdGenerator.NewId())
            .WithContentTypeId(contentTypeId)
            .WithField("title", "Test Title")
            .WithField("body", "Test Body") // Include required field to avoid RequiredFieldMissing error
            .WithField("invalidField", "This field doesn't exist")
            .WithField("anotherInvalidField", 123)
            .Build();

    public static Entry<dynamic> CreateWithoutId(string contentTypeId = "blogPost") =>
        new EntryBuilder()
            .WithContentTypeId(contentTypeId)
            .WithField("title", "Test Title")
            .WithField("body", "Test Body")
            .Build();

    public static Entry<dynamic> CreateWithoutContentType(string? id = null) =>
        new EntryBuilder()
            .WithId(id ?? ContentfulIdGenerator.NewId())
            .WithField("title", "Test Title")
            .WithField("body", "Test Body")
            .Build();

    public static Entry<dynamic> CreateArchivedBlogPost(string? id = null, string contentTypeId = "blogPost") =>
        new EntryBuilder()
            .WithId(id ?? ContentfulIdGenerator.NewId())
            .WithContentTypeId(contentTypeId)
            .WithField("title", "Archived Blog Post")
            .WithField("body", "This is an archived blog post content")
            .WithField("author", "Test Author")
            .AsArchived()
            .Build();

    public static Entry<dynamic> CreateArchivedProduct(string? id = null, string contentTypeId = "product") =>
        new EntryBuilder()
            .WithId(id ?? ContentfulIdGenerator.NewId())
            .WithContentTypeId(contentTypeId)
            .WithField("name", "Archived Product")
            .WithField("price", 19.99)
            .WithField("description", "An archived product")
            .WithField("inStock", false)
            .AsArchived()
            .Build();
}