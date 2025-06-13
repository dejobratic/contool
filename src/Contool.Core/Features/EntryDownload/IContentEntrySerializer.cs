using Contentful.Core.Models;

namespace Contool.Core.Features.EntryDownload;

public interface IContentEntrySerializer
{
    string[] FieldNames { get; }
    dynamic Serialize(Entry<dynamic> entry);
}