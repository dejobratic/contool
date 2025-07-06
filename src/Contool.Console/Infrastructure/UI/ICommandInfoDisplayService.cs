using Contool.Console.Infrastructure.Utils;

namespace Contool.Console.Infrastructure.UI;

public interface ICommandInfoDisplayService
{
    void DisplayCommand(string command, Dictionary<string, object?> options);
    
    void DisplayExecutionMetrics(MeasuredResult<int> result);
}