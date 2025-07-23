using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Infrastructure.Extensions;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Validation;
using Newtonsoft.Json.Linq;

namespace Contool.Core.Features.ContentUpload.Validation;

public static class ValidationRules
{
    public static void ValidateSystemFields(
        Entry<dynamic> entry,
        ContentType contentType,
        int entryIndex,
        ConcurrentHashSet<string> duplicateIds,
        List<ValidationError> errors,
        List<ValidationWarning> warnings)
    {
        var entryId = entry.GetId();
        var entryContentTypeId = entry.GetContentTypeId();

        // Validate sys.id
        if (string.IsNullOrWhiteSpace(entryId))
        {
            var warning = new ValidationWarning(
                entryIndex,
                "sys.id",
                "Missing 'sys.id' - ID will be auto-generated",
                ValidationErrorType.RequiredFieldMissing);

            warnings.Add(warning);
        }
        else if (!duplicateIds.Add(entryId))
        {
            var error = new ValidationError(
                entryIndex,
                "sys.id",
                $"Duplicate ID '{entryId}' found",
                ValidationErrorType.DuplicateId);

            errors.Add(error);
        }

        // Validate sys.contentType
        if (string.IsNullOrWhiteSpace(entryContentTypeId))
        {
            var warning = new ValidationWarning(
                entryIndex,
                "sys.contentType",
                $"Missing 'sys.contentType' - will be set to '{contentType.SystemProperties.Id}'",
                ValidationErrorType.RequiredFieldMissing);

            warnings.Add(warning);
        }
        else if (!string.Equals(entryContentTypeId, contentType.SystemProperties.Id, StringComparison.OrdinalIgnoreCase))
        {
            var error = new ValidationError(
                entryIndex,
                "sys.contentType",
                $"Content type mismatch - expected '{contentType.SystemProperties.Id}', got '{entryContentTypeId}'",
                ValidationErrorType.ContentTypeMismatch);

            errors.Add(error);
        }
    }

    public static void ValidateRequiredFields(
        Entry<dynamic> entry,
        ContentType contentType,
        int entryIndex,
        List<ValidationError> errors)
    {
        var requiredFields = contentType.Fields?.Where(f => f.Required) ?? [];
        var entryFields = entry.Fields as JObject ?? new JObject();

        foreach (var field in requiredFields)
        {
            if (entryFields.TryGetValue(field.Id, out var value)
                && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                continue;
            }

            var error = new ValidationError(
                entryIndex,
                field.Id,
                $"Required field '{field.Name}' ({field.Id}) is missing or empty",
                ValidationErrorType.RequiredFieldMissing);

            errors.Add(error);
        }
    }

    public static void ValidateFieldsExist(
        Entry<dynamic> entry,
        ContentType contentType,
        int entryIndex,
        List<ValidationError> errors)
    {
        var entryFields = entry.Fields as JObject ?? new JObject();
        var validFieldIds = new HashSet<string>(contentType.Fields?.Select(f => f.Id) ?? []);

        foreach (var (fieldId, _) in entryFields)
        {
            if (validFieldIds.Contains(fieldId))
                continue;

            var error = new ValidationError(
                entryIndex,
                fieldId,
                $"Field '{fieldId}' does not exist in content type '{contentType.SystemProperties.Id}'",
                ValidationErrorType.InvalidField);

            errors.Add(error);
        }
    }

    public static void ValidateFieldTypes(
        Entry<dynamic> entry,
        ContentType contentType,
        int entryIndex,
        List<ValidationError> errors)
    {
        var entryFields = entry.Fields as JObject ?? new JObject();
        var fieldMap = contentType.Fields?.ToDictionary(f => f.Id) ?? new Dictionary<string, Field>();

        foreach (var (fieldId, fieldValue) in entryFields)
        {
            if (!fieldMap.TryGetValue(fieldId, out var fieldDefinition))
                continue; // Already handled in ValidateFieldsExist

            if (fieldValue == null)
                continue; // Already handled in ValidateRequiredFields

            var isValid = ContentFieldType.TryFromName(fieldDefinition.Type, out var contentFieldType)
                && contentFieldType!.IsValidRawValue(fieldValue);
            
            if (isValid)
                continue;
            
            var error = new ValidationError(
                entryIndex,
                fieldId,
                $"Field '{fieldDefinition.Name}' ({fieldId}) has invalid type. Expected: {fieldDefinition.Type}, Got: {fieldValue.GetType().Name}",
                ValidationErrorType.InvalidFieldType);

            errors.Add(error);
        }
    }

    public static void ValidateLocales(
        Entry<dynamic> entry,
        IReadOnlyList<Locale> locales,
        int entryIndex,
        List<ValidationError> errors)
    {
        var entryFields = entry.Fields as JObject ?? new JObject();
        var validLocales = new HashSet<string>(locales.Select(l => l.Code));

        foreach (var (fieldId, _) in entryFields)
        {
            if (!fieldId.Contains('.'))
                continue; // Not a localized field

            var contentFieldName = new ContentFieldName(fieldId);
            var locale = contentFieldName.Locale;

            if (validLocales.Contains(locale))
                continue;
            
            var error = new ValidationError(
                entryIndex,
                fieldId,
                $"Field '{fieldId}' uses invalid locale '{locale}'. Valid locales are: {string.Join(", ", validLocales)}",
                ValidationErrorType.InvalidLocale);

            errors.Add(error);
        }
    }
}