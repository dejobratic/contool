using Contool.Core.Features.ContentUpload;
using Contool.Core.Features.ContentUpload.Validation;
using Contool.Core.Infrastructure.Validation;

namespace Contool.Console.Infrastructure.UI.Services;

public class ContentUploadEntryValidatorConsoleDecorator(
    IContentUploadEntryValidator validator,
    IValidationSummaryDisplayService displayService) : IContentUploadEntryValidator
{
    public async Task<EntryValidationSummary> ValidateAsync(
        ContentUploaderInput input,
        CancellationToken cancellationToken)
    {
        var summary = await validator.ValidateAsync(input, cancellationToken);
        
        var totalEntries = summary.ValidEntries.Count + summary.Errors.Select(e => e.EntryIndex).Distinct().Count();
        displayService.DisplayValidationSummary(summary, totalEntries);
        
        return summary;
    }
}