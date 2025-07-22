using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulEntryBulkOperationServiceFactory(
    IOperationTracker operationTracker,
    IProgressReporter progressReporter) : IContentfulEntryBulkOperationServiceFactory
{
    public IContentfulEntryBulkOperationService Create(IContentfulBulkClient client)
    {
        var service = new ContentfulEntryBulkOperationService(client);

        return new ContentfulEntryBulkOperationServiceProgressTrackingDecorator(
            service, operationTracker, progressReporter);
    }
}