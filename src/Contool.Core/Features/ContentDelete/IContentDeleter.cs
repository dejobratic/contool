namespace Contool.Core.Features.ContentDelete;

public interface IContentDeleter
{
    Task DeleteAsync(ContentDeleterInput input, CancellationToken cancellationToken);
}