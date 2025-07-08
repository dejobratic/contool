using Contentful.Core.Models;
using Contool.Core.Features.ContentUpload.Validation;

namespace Contool.Core.Infrastructure.Validation;

public class EntryValidationSummary
{
    public List<Entry<dynamic>> ValidEntries { get; } = [];
    public List<ValidationError> Errors { get; } = [];
}