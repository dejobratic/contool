namespace Contool.Core.Infrastructure.Contentful.Services;

public interface IContentfulEntryBulkOperationServiceFactory
{
    IContentfulEntryBulkOperationService Create(string? spaceId, string? environmentId);
}