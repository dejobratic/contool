using Contool.Core.Infrastructure.Validation;

namespace Contool.Console.Infrastructure.UI.Services;

public interface IValidationSummaryDisplayService
{
    void DisplayValidationSummary(EntryValidationSummary summary, int totalEntries);
}