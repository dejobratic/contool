namespace Contool.Commands;

internal class ContentDeleteCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;

    public bool Force { get; init; } 
}

