namespace Contool.Core.Features.TypeClone;

public interface IContentCloner
{
    Task CloneAsync(ContentClonerInput input, CancellationToken cancellationToken = default);
}