using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public interface IContentfulLoginService
{
    Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken = default);

    Task<Space> GetDefaultSpaceAsync(CancellationToken cancellationToken = default);

    Task<ContentfulEnvironment> GetDefaultEnvironmentAsync(CancellationToken cancellationToken = default);

    Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<ContentTypeExtended>> GetContentTypeExtendedAsync(CancellationToken cancellationToken = default);

    Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
}
