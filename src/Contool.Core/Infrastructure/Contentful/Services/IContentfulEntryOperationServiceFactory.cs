namespace Contool.Core.Infrastructure.Contentful.Services;

public interface IContentfulEntryOperationServiceFactory
{
    IContentfulEntryOperationService Create(IContentfulManagementClientAdapter client);
}