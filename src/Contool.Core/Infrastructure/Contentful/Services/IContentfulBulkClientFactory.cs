namespace Contool.Core.Infrastructure.Contentful.Services;

public interface IContentfulBulkClientFactory
{
    IContentfulBulkClient Create(string? spaceId, string? environmentId);
}