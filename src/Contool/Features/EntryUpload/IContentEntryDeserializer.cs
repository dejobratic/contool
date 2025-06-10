using Contentful.Core.Models;
using Contool.Contentful.Models;

namespace Contool.Features.EntryUpload;

internal interface IContentEntryDeserializer
{
    Entry<dynamic> Deserialize(ContentFieldName[] headings, dynamic value);
}