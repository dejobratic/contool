using Contentful.Core.Models;

namespace Contool.Features.EntryDownload;

public interface IContentEntrySerializer
{
    string[] FieldNames { get; }
    dynamic Serialize(Entry<dynamic> entry);
}