namespace Contool.Core.Features;

public class CommandBase
{
    public string? SpaceId { get; init; }

    public string? EnvironmentId { get; init; }
}


public interface ICommandHandler<TCommand>
    where TCommand : CommandBase
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}