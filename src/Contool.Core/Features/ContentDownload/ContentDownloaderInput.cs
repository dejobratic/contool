using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;

namespace Contool.Core.Features.ContentDownload;

public class ContentDownloaderInput
{
    public string ContentTypeId { get; init; } = null!;
    
    public OutputContent Output { get; init; } = null!;
    
    public IOutputWriter OutputWriter { get; init; } = null!;
}