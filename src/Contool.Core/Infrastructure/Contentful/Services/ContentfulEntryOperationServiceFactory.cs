using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulEntryOperationServiceFactory(
    IProgressReporter progressReporter) : IContentfulEntryOperationServiceFactory
{
    public IContentfulEntryOperationService Create(IContentfulManagementClientAdapter client)
    {
        var service = new ContentfulEntryOperationService(client);
        
        return new ContentfulEntryOperationServiceProgressTrackingDecorator(
            service, progressReporter);
    }
}