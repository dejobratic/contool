using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Console.Infrastructure.UI;

public interface IContentfulInfoDisplayService
{
    Task DisplayInfoAsync(IContentfulLoginService contentfulService);
}