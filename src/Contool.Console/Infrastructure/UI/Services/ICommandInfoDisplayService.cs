using Contool.Console.Infrastructure.Utils;
using Contool.Console.Infrastructure.Utils.Models;

namespace Contool.Console.Infrastructure.UI.Services;

public interface ICommandInfoDisplayService
{
    void DisplayCommand(string command, Dictionary<string, object?> options);
    
    void DisplayExecutionMetrics(MeasuredResult<int> result);
}