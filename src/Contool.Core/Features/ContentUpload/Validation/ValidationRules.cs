using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Validation;
using Microsoft.Extensions.Logging;
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
        ILogger logger)
    {
        var entryId = entry.GetId();
        var entryContentTypeId = entry.GetContentTypeId();

        // Validate sys.id
        if (string.IsNullOrWhiteSpace(entryId))
        {
            logger.LogInformation(
                "Entry #{EntryIndex}: Missing 'sys.id' - ID will be auto-generated",
                entryIndex + 1);
        }
        else if (!duplicateIds.Add(entryId))
        {
            var error = new ValidationError(
                entryIndex,
                "sys.id",
                $"Duplicate ID '{entryId}' found",
                ValidationErrorType.DuplicateId);

            errors.Add(error);

            logger.LogWarning(
                "Entry #{EntryIndex}: Duplicate ID '{EntryId}' detected",
                entryIndex + 1, entryId);
        }

        // Validate sys.contentType
        if (string.IsNullOrWhiteSpace(entryContentTypeId))
        {
            logger.LogInformation(
                "Entry #{EntryIndex}: Missing 'sys.contentType' - will be set to '{ExpectedType}'",
                entryIndex + 1, contentType.SystemProperties.Id);
        }
        else if (!string.Equals(entryContentTypeId, contentType.SystemProperties.Id, StringComparison.OrdinalIgnoreCase))
        {
            var error = new ValidationError(
                entryIndex,
                "sys.contentType",
                $"Content type mismatch - expected '{contentType.SystemProperties.Id}', got '{entryContentTypeId}'",
                ValidationErrorType.ContentTypeMismatch);

            errors.Add(error);

            logger.LogWarning(
                "Entry #{EntryIndex}: Content type mismatch - expected '{ExpectedType}', got '{ActualType}'",
                entryIndex + 1, contentType.SystemProperties.Id, entryContentTypeId);
        }
    }

    public static void ValidateRequiredFields(
        Entry<dynamic> entry,
        ContentType contentType,
        int entryIndex,
        List<ValidationError> errors,
        ILogger logger)
    {
        var requiredFields = contentType.Fields?.Where(f => f.Required) ?? [];
        var entryFields = entry.Fields as JObject ?? new JObject();

        foreach (var field in requiredFields)
        {
            if (entryFields.TryGetValue(field.Id, out var value)
                && value is not null
                && (value is not JToken str || !string.IsNullOrWhiteSpace(str.ToString())))
            {
                continue;
            }

            var error = new ValidationError(
                entryIndex,
                field.Id,
                $"Required field '{field.Name}' ({field.Id}) is missing or empty",
                ValidationErrorType.RequiredFieldMissing);

            errors.Add(error);

            logger.LogWarning(
                "Entry #{EntryIndex}: Required field '{FieldName}' ({FieldId}) is missing or empty",
                entryIndex + 1, field.Name, field.Id);
        }
    }

    public static void ValidateFieldsExist(
        Entry<dynamic> entry,
        ContentType contentType,
        int entryIndex,
        List<ValidationError> errors,
        ILogger logger)
    {
        var entryFields = entry.Fields as IDictionary<string, object?> ?? new Dictionary<string, object?>();
        var validFieldIds = new HashSet<string>(contentType.Fields?.Select(f => f.Id) ?? []);

        foreach (var entryField in entryFields)
        {
            if (validFieldIds.Contains(entryField.Key))
                continue;

            var error = new ValidationError(
                entryIndex,
                entryField.Key,
                $"Field '{entryField.Key}' does not exist in content type '{contentType.SystemProperties.Id}'",
                ValidationErrorType.InvalidField);

            errors.Add(error);

            logger.LogWarning(
                "Entry #{EntryIndex}: Field '{FieldId}' does not exist in content type '{ContentTypeId}'",
                entryIndex + 1, entryField.Key, contentType.SystemProperties.Id);
        }
    }

    public static void ValidateFieldTypes(
        Entry<dynamic> entry,
        ContentType contentType,
        int entryIndex,
        List<ValidationError> errors,
        ILogger logger)
    {
        var entryFields = entry.Fields as IDictionary<string, object?> ?? new Dictionary<string, object?>();
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

            logger.LogWarning(
                "Entry #{EntryIndex}: Field '{FieldName}' ({FieldId}) has invalid type. Expected: {ExpectedType}, Got: {ActualType}",
                entryIndex + 1, fieldDefinition.Name, fieldId, fieldDefinition.Type,
                fieldValue.GetType().Name);
        }
    }
}