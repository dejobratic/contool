using Contentful.Core.Models;

namespace Contool.Contentful.Services;

public interface IContentEntrySerializer
{
    string[] FieldNames { get; }
    dynamic Serialize(Entry<dynamic> entry);
}