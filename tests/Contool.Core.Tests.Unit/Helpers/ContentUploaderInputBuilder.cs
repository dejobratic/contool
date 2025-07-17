using Contentful.Core.Models;
using Contool.Core.Features.ContentUpload;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Tests.Unit.Helpers;

public class ContentUploaderInputBuilder
{
    private string _contentTypeId = "blogPost";
    private IContentfulService _contentfulService;
    private IAsyncEnumerableWithTotal<Entry<dynamic>> _entries;
    private bool _uploadOnlyValidEntries = false;
    private bool _publishEntries = false;

    public ContentUploaderInputBuilder WithContentTypeId(string contentTypeId)
    {
        _contentTypeId = contentTypeId;
        return this;
    }

    public ContentUploaderInputBuilder WithContentfulService(IContentfulService contentfulService)
    {
        _contentfulService = contentfulService;
        return this;
    }

    public ContentUploaderInputBuilder WithEntries(IAsyncEnumerableWithTotal<Entry<dynamic>> entries)
    {
        _entries = entries;
        return this;
    }

    public ContentUploaderInputBuilder WithEntries(params Entry<dynamic>[] entries)
    {
        _entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries);
        return this;
    }

    public ContentUploaderInputBuilder WithUploadOnlyValidEntries(bool uploadOnlyValidEntries)
    {
        _uploadOnlyValidEntries = uploadOnlyValidEntries;
        return this;
    }

    public ContentUploaderInputBuilder WithPublishEntries(bool publishEntries)
    {
        _publishEntries = publishEntries;
        return this;
    }

    public ContentUploaderInput Build()
    {
        return new ContentUploaderInput
        {
            ContentTypeId = _contentTypeId,
            ContentfulService = _contentfulService ?? throw new InvalidOperationException("ContentfulService is required"),
            Entries = _entries ?? new MockAsyncEnumerableWithTotal<Entry<dynamic>>(Array.Empty<Entry<dynamic>>()),
            UploadOnlyValidEntries = _uploadOnlyValidEntries,
            PublishEntries = _publishEntries
        };
    }

    public static ContentUploaderInputBuilder Create() => new();

    public static ContentUploaderInput CreateDefault(IContentfulService contentfulService, params Entry<dynamic>[] entries)
    {
        return new ContentUploaderInputBuilder()
            .WithContentfulService(contentfulService)
            .WithEntries(entries)
            .Build();
    }

    public static ContentUploaderInput CreateForBlogPost(IContentfulService contentfulService, params Entry<dynamic>[] entries)
    {
        return new ContentUploaderInputBuilder()
            .WithContentTypeId("blogPost")
            .WithContentfulService(contentfulService)
            .WithEntries(entries)
            .Build();
    }

    public static ContentUploaderInput CreateWithValidation(IContentfulService contentfulService, bool uploadOnlyValid, params Entry<dynamic>[] entries)
    {
        return new ContentUploaderInputBuilder()
            .WithContentfulService(contentfulService)
            .WithEntries(entries)
            .WithUploadOnlyValidEntries(uploadOnlyValid)
            .Build();
    }

    public static ContentUploaderInput CreateWithPublishing(IContentfulService contentfulService, bool publishEntries, params Entry<dynamic>[] entries)
    {
        return new ContentUploaderInputBuilder()
            .WithContentfulService(contentfulService)
            .WithEntries(entries)
            .WithPublishEntries(publishEntries)
            .Build();
    }
}