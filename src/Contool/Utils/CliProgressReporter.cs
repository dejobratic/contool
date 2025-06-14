using Contool.Core.Infrastructure.Utils;
using Spectre.Console;

namespace Contool.Utils;

public class CliProgressReporter : IProgressReporter
{
    public void Report(int current, int total)
    {
        Console.Write($"\rProgress: {current}/{total} ({(double)current / total:P2})");

        if (current == total)
        {
            Console.WriteLine(); // Move to the next line when complete
            return;
        }
    }
}
