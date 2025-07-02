using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulEntryOperationServiceFactory(
    IOperationTracker operationTracker,
    IProgressReporter progressReporter) : IContentfulEntryOperationServiceFactory
{
    public IContentfulEntryOperationService Create(IContentfulManagementClientAdapter client)
    {
        var service = new ContentfulEntryOperationService(client);
        
        return new ContentfulEntryOperationServiceProgressTrackingDecorator(
            service, operationTracker, progressReporter);
    }
}