using Contool.Contentful.Models;
using Contentful.Core.Models;

namespace Contool.Contentful.Services;

internal interface IContentEntryDeserializer
{
    Entry<dynamic> Deserialize(ContentFieldName[] headings, dynamic value);
}