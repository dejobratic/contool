using Contentful.Core.Models;

namespace Contool.Core.Features.EntryUpload;

public interface IContentEntryDeserializer
{
    Entry<dynamic> Deserialize(dynamic value);
}