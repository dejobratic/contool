using Contentful.Core.Models;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Validation;
using Microsoft.Extensions.Logging;

namespace Contool.Core.Features.ContentUpload.Validation;

public class ContentUploadEntryValidator(
    ILogger<ContentUploadEntryValidator> logger) : IContentUploadEntryValidator
{
    public async Task<EntryValidationSummary> ValidateAsync(
        ContentUploaderInput input,
        CancellationToken cancellationToken)
    {
        var summary = new EntryValidationSummary();
        var duplicateIds = new ConcurrentHashSet<string>();
        
        var contentType = await input.ContentfulService
            .GetContentTypeAsync(input.ContentTypeId, cancellationToken);

        if (contentType == null)
        {
            logger.LogError("Content type '{ContentTypeId}' not found. Cannot validate entries.", input.ContentTypeId);
            return summary;
        }

        logger.LogInformation("Starting validation for content type '{ContentTypeId}'", input.ContentTypeId);

        var index = 0;

        await foreach (var entry in input.Entries.WithCancellation(cancellationToken))
        {
            var result = ValidateEntry(entry, contentType, index, duplicateIds);
            
            if (result.IsValid)
            {
                summary.ValidEntries.Add(entry);
            }
            else
            {
                summary.Errors.AddRange(result.Errors);
            }

            index++;
        }

        LogValidationSummary(summary, index);
        return summary;
    }

    private EntryValidationResult ValidateEntry(
        Entry<dynamic> entry,
        ContentType contentType,
        int index,
        ConcurrentHashSet<string> duplicateIds)
    {
        var errors = new List<ValidationError>();

        ValidationRules.ValidateSystemFields(entry, contentType, index, duplicateIds, errors, logger);
        ValidationRules.ValidateRequiredFields(entry, contentType, index, errors, logger);
        ValidationRules.ValidateFieldsExist(entry, contentType, index, errors, logger);
        ValidationRules.ValidateFieldTypes(entry, contentType, index, errors, logger);

        return new EntryValidationResult(errors.Count == 0, errors);
    }

    private void LogValidationSummary(EntryValidationSummary summary, int total)
    {
        if (summary.Errors.Count == 0)
        {
            logger.LogInformation("Validation completed successfully - {ValidCount}/{TotalCount} entries are valid",
                summary.ValidEntries.Count, total);
        }
        else
        {
            logger.LogWarning("Validation completed with {ErrorCount} errors - {ValidCount}/{TotalCount} entries are valid.",
                summary.Errors.Count, summary.ValidEntries.Count, total);

            foreach (var errorGroup in summary.Errors.GroupBy(e => e.Type))
            {
                logger.LogWarning("  - {ErrorType}: {Count} errors", errorGroup.Key, errorGroup.Count());
            }
        }
    }
}