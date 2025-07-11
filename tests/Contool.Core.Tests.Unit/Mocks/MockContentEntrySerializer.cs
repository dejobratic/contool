using Contentful.Core.Models;
using Contool.Core.Features.ContentDownload;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentEntrySerializer : IContentEntrySerializer
{
    public bool SerializeWasCalled { get; private set; }
    public Entry<dynamic>? LastEntry { get; private set; }

    public string[] FieldNames { get; } = ["id", "createdAt", "updatedAt"];

    public dynamic Serialize(Entry<dynamic> entry)
    {
        SerializeWasCalled = true;
        LastEntry = entry;
        
        return new Dictionary<string, object?>
        {
            ["id"] = entry.SystemProperties?.Id,
            ["createdAt"] = entry.SystemProperties?.CreatedAt,
            ["updatedAt"] = entry.SystemProperties?.UpdatedAt
        };
    }
}