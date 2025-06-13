using Contentful.Core.Models;
using Contool.Core.Contentful.Models;

namespace Contool.Core.Features.EntryUpload;

public interface IContentEntryDeserializer
{
    Entry<dynamic> Deserialize(ContentFieldName[] headings, dynamic value);
}