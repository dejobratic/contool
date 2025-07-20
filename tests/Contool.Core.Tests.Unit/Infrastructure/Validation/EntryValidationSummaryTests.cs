using Contool.Core.Infrastructure.Validation;
using Contool.Core.Tests.Unit.Helpers;

namespace Contool.Core.Tests.Unit.Infrastructure.Validation;

public class EntryValidationSummaryTests
{
    [Fact]
    public void GivenNewSummary_WhenCreated_ThenAllCollectionsAreEmpty()
    {
        // Act
        var summary = new EntryValidationSummary();

        // Assert
        Assert.Empty(summary.ValidEntries);
        Assert.Empty(summary.Errors);
        Assert.Empty(summary.Warnings);
        Assert.NotNull(summary.ValidEntries);
        Assert.NotNull(summary.Errors);
        Assert.NotNull(summary.Warnings);
    }

    [Fact]
    public void GivenSummary_WhenAddingValidEntry_ThenValidEntriesCollectionContainsEntry()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        var entry = EntryBuilder.CreateBlogPost();

        // Act
        summary.ValidEntries.Add(entry);

        // Assert
        Assert.Single(summary.ValidEntries);
        Assert.Contains(entry, summary.ValidEntries);
        Assert.Empty(summary.Errors);
        Assert.Empty(summary.Warnings);
    }

    [Fact]
    public void GivenSummary_WhenAddingMultipleValidEntries_ThenAllEntriesArePresent()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        
        var entry1 = EntryBuilder.CreateBlogPost();
        var entry2 = EntryBuilder.CreateProduct();
        var entry3 = EntryBuilder.CreateMinimal();

        // Act
        summary.ValidEntries.Add(entry1);
        summary.ValidEntries.Add(entry2);
        summary.ValidEntries.Add(entry3);

        // Assert
        Assert.Equal(3, summary.ValidEntries.Count);
        Assert.Contains(entry1, summary.ValidEntries);
        Assert.Contains(entry2, summary.ValidEntries);
        Assert.Contains(entry3, summary.ValidEntries);
    }

    [Fact]
    public void GivenSummary_WhenAddingError_ThenErrorsCollectionContainsError()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        
        var error = ValidationErrorBuilder.CreateRequiredFieldMissing();

        // Act
        summary.Errors.Add(error);

        // Assert
        Assert.Single(summary.Errors);
        Assert.Contains(error, summary.Errors);
        Assert.Empty(summary.ValidEntries);
        Assert.Empty(summary.Warnings);
    }

    [Fact]
    public void GivenSummary_WhenAddingMultipleErrors_ThenAllErrorsArePresent()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        
        var error1 = ValidationErrorBuilder.CreateRequiredFieldMissing();
        var error2 = ValidationErrorBuilder.CreateDuplicateId(1, "duplicate-id");
        var error3 = ValidationErrorBuilder.CreateInvalidField(2, "unknownField");

        // Act
        summary.Errors.Add(error1);
        summary.Errors.Add(error2);
        summary.Errors.Add(error3);

        // Assert
        Assert.Equal(3, summary.Errors.Count);
        Assert.Contains(error1, summary.Errors);
        Assert.Contains(error2, summary.Errors);
        Assert.Contains(error3, summary.Errors);
    }

    [Fact]
    public void GivenSummary_WhenAddingWarning_ThenWarningsCollectionContainsWarning()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        var warning = ValidationWarningBuilder.CreateMissingId(0);

        // Act
        summary.Warnings.Add(warning);

        // Assert
        Assert.Single(summary.Warnings);
        Assert.Contains(warning, summary.Warnings);
        Assert.Empty(summary.ValidEntries);
        Assert.Empty(summary.Errors);
    }

    [Fact]
    public void GivenSummary_WhenAddingMultipleWarnings_ThenAllWarningsArePresent()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        var warning1 = ValidationWarningBuilder.CreateMissingId(0);
        var warning2 = ValidationWarningBuilder.CreateMissingContentType(1, "blogPost");
        var warning3 = ValidationWarningBuilder.Create()
            .WithEntryIndex(2)
            .WithFieldId("deprecatedField")
            .WithMessage("Field is deprecated")
            .WithType(ValidationErrorType.InvalidField)
            .Build();

        // Act
        summary.Warnings.Add(warning1);
        summary.Warnings.Add(warning2);
        summary.Warnings.Add(warning3);

        // Assert
        Assert.Equal(3, summary.Warnings.Count);
        Assert.Contains(warning1, summary.Warnings);
        Assert.Contains(warning2, summary.Warnings);
        Assert.Contains(warning3, summary.Warnings);
    }

    [Fact]
    public void GivenSummary_WhenAddingMixedValidationResults_ThenAllCollectionsContainCorrectItems()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        var validEntry1 = EntryBuilder.CreateBlogPost("valid1", "blogPost");
        var validEntry2 = EntryBuilder.CreateProduct("valid2", "product");
        var error1 = ValidationErrorBuilder.CreateRequiredFieldMissing(2, "title");
        var error2 = ValidationErrorBuilder.CreateDuplicateId(3, "dup-id");
        var warning1 = ValidationWarningBuilder.CreateMissingId(4);
        var warning2 = ValidationWarningBuilder.CreateMissingContentType(5, "blogPost");

        // Act
        summary.ValidEntries.Add(validEntry1);
        summary.ValidEntries.Add(validEntry2);
        summary.Errors.Add(error1);
        summary.Errors.Add(error2);
        summary.Warnings.Add(warning1);
        summary.Warnings.Add(warning2);

        // Assert
        Assert.Equal(2, summary.ValidEntries.Count);
        Assert.Equal(2, summary.Errors.Count);
        Assert.Equal(2, summary.Warnings.Count);

        Assert.Contains(validEntry1, summary.ValidEntries);
        Assert.Contains(validEntry2, summary.ValidEntries);
        Assert.Contains(error1, summary.Errors);
        Assert.Contains(error2, summary.Errors);
        Assert.Contains(warning1, summary.Warnings);
        Assert.Contains(warning2, summary.Warnings);
    }

    [Fact]
    public void GivenSummary_WhenClearingCollections_ThenAllCollectionsBecomeEmpty()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        summary.ValidEntries.Add(EntryBuilder.CreateDefault());
        summary.Errors.Add(ValidationErrorBuilder.CreateDefault());
        summary.Warnings.Add(ValidationWarningBuilder.CreateDefault());

        // Act
        summary.ValidEntries.Clear();
        summary.Errors.Clear();
        summary.Warnings.Clear();

        // Assert
        Assert.Empty(summary.ValidEntries);
        Assert.Empty(summary.Errors);
        Assert.Empty(summary.Warnings);
    }

    [Fact]
    public void GivenSummary_WhenRemovingSpecificItems_ThenOnlySpecifiedItemsAreRemoved()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        var entry1 = EntryBuilder.CreateBlogPost("keep", "blogPost");
        var entry2 = EntryBuilder.CreateProduct("remove", "product");
        var error1 = ValidationErrorBuilder.CreateRequiredFieldMissing(0, "keep");
        var error2 = ValidationErrorBuilder.CreateDuplicateId(1, "remove");
        var warning1 = ValidationWarningBuilder.CreateMissingId(0);
        var warning2 = ValidationWarningBuilder.CreateMissingContentType(1, "blogPost");

        summary.ValidEntries.Add(entry1);
        summary.ValidEntries.Add(entry2);
        summary.Errors.Add(error1);
        summary.Errors.Add(error2);
        summary.Warnings.Add(warning1);
        summary.Warnings.Add(warning2);

        // Act
        summary.ValidEntries.Remove(entry2);
        summary.Errors.Remove(error2);
        summary.Warnings.Remove(warning2);

        // Assert
        Assert.Single(summary.ValidEntries);
        Assert.Single(summary.Errors);
        Assert.Single(summary.Warnings);
        Assert.Contains(entry1, summary.ValidEntries);
        Assert.Contains(error1, summary.Errors);
        Assert.Contains(warning1, summary.Warnings);
    }

    [Fact]
    public void GivenEmptySummary_WhenCheckingCounts_ThenAllCountsAreZero()
    {
        // Arrange
        var summary = new EntryValidationSummary();

        // Assert
        Assert.Empty(summary.ValidEntries);
        Assert.Empty(summary.Errors);
        Assert.Empty(summary.Warnings);
    }

    [Fact]
    public void GivenSummaryWithItems_WhenCheckingCounts_ThenCountsReflectActualItems()
    {
        // Arrange
        var summary = new EntryValidationSummary();

        // Act
        for (int i = 0; i < 5; i++)
        {
            summary.ValidEntries.Add(EntryBuilder.CreateBlogPost($"entry{i}", "blogPost"));
        }
        for (int i = 0; i < 3; i++)
        {
            summary.Errors.Add(ValidationErrorBuilder.CreateRequiredFieldMissing(i, $"field{i}"));
        }
        for (int i = 0; i < 2; i++)
        {
            summary.Warnings.Add(ValidationWarningBuilder.CreateMissingId(i));
        }

        // Assert
        Assert.Equal(5, summary.ValidEntries.Count);
        Assert.Equal(3, summary.Errors.Count);
        Assert.Equal(2, summary.Warnings.Count);
    }

    [Fact]
    public void GivenSummary_WhenIteratingThroughCollections_ThenAllItemsAreAccessible()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateProduct("entry2", "product")
        };
        var errors = new[]
        {
            ValidationErrorBuilder.CreateRequiredFieldMissing(0, "field1"),
            ValidationErrorBuilder.CreateDuplicateId(1, "dup-id")
        };
        var warnings = new[]
        {
            ValidationWarningBuilder.CreateMissingId(0),
            ValidationWarningBuilder.CreateMissingContentType(1, "blogPost")
        };

        foreach (var entry in entries) summary.ValidEntries.Add(entry);
        foreach (var error in errors) summary.Errors.Add(error);
        foreach (var warning in warnings) summary.Warnings.Add(warning);

        // Act & Assert
        var validEntriesList = summary.ValidEntries.ToList();
        var errorsList = summary.Errors.ToList();
        var warningsList = summary.Warnings.ToList();

        Assert.Equal(2, validEntriesList.Count);
        Assert.Equal(2, errorsList.Count);
        Assert.Equal(2, warningsList.Count);

        Assert.All(entries, entry => Assert.Contains(entry, validEntriesList));
        Assert.All(errors, error => Assert.Contains(error, errorsList));
        Assert.All(warnings, warning => Assert.Contains(warning, warningsList));
    }
}