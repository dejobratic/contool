using Contentful.Core;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using System.Runtime.CompilerServices;

namespace Contool.Contentful.Services;

internal class ContentfulService(IContentfulManagementClient client) : IContentfulService
{
    public async Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken)
    {
        return await client.GetLocalesCollection(cancellationToken: cancellationToken);
    }

    public async Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        try
        {
            return await client.GetContentType(contentTypeId, cancellationToken: cancellationToken);
        }
        catch(ContentfulException)
        {
            return null;
        }
    }
    
    public async Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default)
    {
        return await client.GetContentTypes(cancellationToken: cancellationToken);
    }

    public async Task<ContentType> CreateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default)
    {
        var existingContentType = await  GetContentTypeAsync(
            contentType.SystemProperties.Id,
            cancellationToken);

        if (existingContentType is not null)
            throw new ArgumentException($"Content type with ID '{contentType.SystemProperties.Id}' already exists.");

        var created =  await client.CreateOrUpdateContentType(
            contentType,
            cancellationToken: cancellationToken);
        
        created = await client.ActivateContentType(
            contentTypeId: created.SystemProperties.Id,
            version: created.SystemProperties.Version!.Value,
            cancellationToken: cancellationToken);

        return created;
    }

    public async IAsyncEnumerable<Entry<dynamic>> GetEntriesAsync(string contentTypeId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int skip = 0;
        int pageSize = 100;
        bool moreResults = true;

        var query = new QueryBuilder<dynamic>()
            .ContentTypeIs(contentTypeId.ToCamelCase());

        while (moreResults)
        {
            var queryString = query
                .Limit(pageSize)
                .Skip(skip)
                .Build();

            var entries = await client
                .GetEntriesCollection<Entry<dynamic>>(queryString, cancellationToken: cancellationToken);

            if (entries == null || !entries.Any())
            {
                yield break;
            }

            foreach (var entry in entries)
            {
                yield return entry;
            }

            if (entries.Count() < pageSize)
            {
                moreResults = false;
            }
            else
            {
                skip += pageSize;
            }
        }
    }

    public async Task UpsertEntriesAsync(IEnumerable<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default)
    {
        foreach (var entry in entries)
        {
            await client.CreateOrUpdateEntry(
                entry.Fields,
                entry.SystemProperties?.Id,
                contentTypeId: entry.SystemProperties?.ContentType?.SystemProperties?.Id,
                version: entry.SystemProperties?.Version ?? 0,
                cancellationToken: cancellationToken);

            if(publish)
            {
                await client.PublishEntry(
                    entryId: entry.SystemProperties?.Id,
                    version: entry.SystemProperties?.Version ?? 0,
                    cancellationToken: cancellationToken);
            }
        }
    }

    public async Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        foreach (var entry in entries)
        {
            await client.PublishEntry(
                entryId: entry.SystemProperties?.Id,
                version: entry.SystemProperties?.Version ?? 0,
                cancellationToken: cancellationToken);
        }
    }
}