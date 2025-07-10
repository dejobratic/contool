using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Console.Infrastructure.UI.Services;

public interface IContentfulInfoDisplayService
{
    Task DisplayInfoAsync(IContentfulLoginService contentfulService);
}