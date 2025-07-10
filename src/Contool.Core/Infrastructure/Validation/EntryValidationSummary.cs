using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Validation;

public class EntryValidationSummary
{
    public List<Entry<dynamic>> ValidEntries { get; } = [];
    public List<ValidationError> Errors { get; } = [];
    public List<ValidationWarning> Warnings { get; } = [];
}