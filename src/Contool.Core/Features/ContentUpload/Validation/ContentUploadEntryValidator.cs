using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Validation;
using Microsoft.Extensions.Logging;

namespace Contool.Core.Features.ContentUpload.Validation;

public class ContentUploadEntryValidator : IContentUploadEntryValidator
{
    public async Task<EntryValidationSummary> ValidateAsync(
        ContentUploaderInput input,
        CancellationToken cancellationToken)
    {
        var summary = new EntryValidationSummary();
        var duplicateIds = new ConcurrentHashSet<string>();
        
        var contentType = (await input.ContentfulService
            .GetContentTypeAsync(input.ContentTypeId, cancellationToken))!;
        
        var locales = (await input.ContentfulService
            .GetLocalesAsync(cancellationToken))
            .ToList();

        var index = 0;

        await foreach (var entry in input.Entries.WithCancellation(cancellationToken))
        {
            var result = ValidateEntry(entry, contentType, locales, index, duplicateIds, summary.Warnings);
            
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

        return summary;
    }

    private static EntryValidationResult ValidateEntry(
        Entry<dynamic> entry,
        ContentType contentType,
        IReadOnlyList<Locale> locales,
        int index,
        ConcurrentHashSet<string> duplicateIds,
        List<ValidationWarning> warnings)
    {
        var errors = new List<ValidationError>();

        ValidationRules.ValidateSystemFields(entry, contentType, index, duplicateIds, errors, warnings);
        ValidationRules.ValidateRequiredFields(entry, contentType, index, errors);
        ValidationRules.ValidateFieldsExist(entry, contentType, index, errors);
        ValidationRules.ValidateFieldTypes(entry, contentType, index, errors);
        ValidationRules.ValidateLocales(entry, locales, index, errors);

        return new EntryValidationResult(errors.Count == 0, errors);
    }
}