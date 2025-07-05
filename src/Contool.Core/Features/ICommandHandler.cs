namespace Contool.Core.Features;

public interface ICommandHandler<in TCommand>
    where TCommand : CommandBase
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}