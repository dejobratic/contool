using Contentful.Core.Models;

namespace Contool.Core.Features.ContentUpload;

public interface IContentEntryDeserializer
{
    Entry<dynamic> Deserialize(dynamic value);
}