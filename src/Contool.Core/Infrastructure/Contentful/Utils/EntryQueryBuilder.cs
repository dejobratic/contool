using Contentful.Core.Models;
using Contentful.Core.Search;

namespace Contool.Core.Infrastructure.Contentful.Utils;

public class EntryQueryBuilder
{
    private string? _contentTypeId;
    private int _skip = 0;
    private int _limit = 100;

    public EntryQueryBuilder WithContentTypeId(string contentTypeId)
    {
        _contentTypeId = contentTypeId;
        return this;
    }

    public EntryQueryBuilder Skip(int skip)
    {
        _skip = skip;
        return this;
    }

    public EntryQueryBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    public string Build()
    {
        return QueryBuilder<Entry<dynamic>>.New
            .ContentTypeIs(_contentTypeId)
            .Skip(_skip)
            .Limit(_limit)
            .Build();
    }
}
