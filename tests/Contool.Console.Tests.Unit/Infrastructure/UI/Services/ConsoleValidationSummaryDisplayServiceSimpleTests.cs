using Contool.Console.Infrastructure.UI.Services;
using Contool.Core.Infrastructure.Validation;
using Contool.Core.Tests.Unit.Helpers;

namespace Contool.Console.Tests.Unit.Infrastructure.UI.Services;

public class ConsoleValidationSummaryDisplayServiceSimpleTests
{
    private readonly ConsoleValidationSummaryDisplayService _service = new();

    [Fact]
    public void GivenEmptySummary_WhenDisplayed_ThenNoExceptionIsThrown()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        
        const int totalEntries = 0;

        // Act & Assert - Should not throw
        _service.DisplayValidationSummary(summary, totalEntries);
    }

    [Fact]
    public void GivenSummaryWithValidEntriesOnly_WhenDisplayed_ThenNoExceptionIsThrown()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        summary.ValidEntries.Add(EntryBuilder.CreateBlogPost("entry1"));
        summary.ValidEntries.Add(EntryBuilder.CreateProduct("entry2"));
        
        const int totalEntries = 2;

        // Act & Assert - Should not throw
        _service.DisplayValidationSummary(summary, totalEntries);
    }

    [Fact]
    public void GivenSummaryWithErrors_WhenDisplayed_ThenNoExceptionIsThrown()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        summary.Errors.Add(ValidationErrorBuilder.CreateRequiredFieldMissing(0, "title"));
        summary.Errors.Add(ValidationErrorBuilder.CreateDuplicateId(1, "duplicate-id"));
        
        const int totalEntries = 2;

        // Act & Assert - Should not throw
        _service.DisplayValidationSummary(summary, totalEntries);
    }

    [Fact]
    public void GivenSummaryWithWarnings_WhenDisplayed_ThenNoExceptionIsThrown()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        summary.Warnings.Add(ValidationWarningBuilder.CreateMissingId(0));
        summary.Warnings.Add(ValidationWarningBuilder.CreateMissingContentType(1, "blogPost"));
        
        const int totalEntries = 2;

        // Act & Assert - Should not throw
        _service.DisplayValidationSummary(summary, totalEntries);
    }

    [Fact]
    public void GivenSummaryWithBothErrorsAndWarnings_WhenDisplayed_ThenNoExceptionIsThrown()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        summary.ValidEntries.Add(EntryBuilder.CreateBlogPost());
        summary.Errors.Add(ValidationErrorBuilder.CreateInvalidField(1, "unknownField"));
        summary.Warnings.Add(ValidationWarningBuilder.CreateMissingId(2));
        
        const int totalEntries = 3;

        // Act & Assert - Should not throw
        _service.DisplayValidationSummary(summary, totalEntries);
    }

    [Fact]
    public void GivenSummaryWithAllValidationErrorTypes_WhenDisplayed_ThenNoExceptionIsThrown()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        summary.Errors.Add(new ValidationError(0, "field1", "Error 1", ValidationErrorType.DuplicateId));
        summary.Errors.Add(new ValidationError(1, "field2", "Error 2", ValidationErrorType.ContentTypeMismatch));
        summary.Errors.Add(new ValidationError(2, "field3", "Error 3", ValidationErrorType.RequiredFieldMissing));
        summary.Errors.Add(new ValidationError(3, "field4", "Error 4", ValidationErrorType.InvalidField));
        summary.Errors.Add(new ValidationError(4, "field5", "Error 5", ValidationErrorType.InvalidFieldType));
        summary.Errors.Add(new ValidationError(5, "field6", "Error 6", ValidationErrorType.InvalidFieldValue));
        summary.Errors.Add(new ValidationError(6, "field7", "Error 7", ValidationErrorType.InvalidLocale));
        
        const int totalEntries = 7;

        // Act & Assert - Should not throw
        _service.DisplayValidationSummary(summary, totalEntries);
    }

    [Fact]
    public void GivenLargeNumberOfEntries_WhenDisplayed_ThenNoExceptionIsThrown()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        
        for (var i = 0; i < 100; i++) // Reduce number for performance
            summary.ValidEntries.Add(EntryBuilder.CreateBlogPost($"entry{i}"));

        for (var i = 0; i < 10; i++)
            summary.Errors.Add(ValidationErrorBuilder.CreateRequiredFieldMissing(100 + i, $"field{i}"));

        const int totalEntries = 110;

        // Act & Assert - Should not throw
        _service.DisplayValidationSummary(summary, totalEntries);
    }

    [Fact]
    public void GivenNullSummary_WhenDisplayed_ThenNullReferenceExceptionIsThrown()
    {
        // Arrange
        EntryValidationSummary? summary = null;
        
        const int totalEntries = 0;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => _service.DisplayValidationSummary(summary!, totalEntries));
    }

    [Fact]
    public void GivenNegativeTotalEntries_WhenDisplayed_ThenNoExceptionIsThrown()
    {
        // Arrange
        var summary = new EntryValidationSummary();
        
        const int totalEntries = -1;

        // Act & Assert - Should handle edge case gracefully
        _service.DisplayValidationSummary(summary, totalEntries);
    }

    [Fact]
    public void GivenServiceImplementsInterface_WhenChecked_ThenImplementsIValidationSummaryDisplayService()
    {
        // Arrange & Act
        var implementsInterface = _service is IValidationSummaryDisplayService service;

        // Assert
        Assert.True(implementsInterface);
    }
}