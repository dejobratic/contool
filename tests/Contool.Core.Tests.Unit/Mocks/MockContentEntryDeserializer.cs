using Contentful.Core.Models;
using Contool.Core.Features.ContentUpload;
using Contool.Core.Tests.Unit.Helpers;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentEntryDeserializer : IContentEntryDeserializer
{
    public int DeserializeCallCount { get; private set; }
    public List<dynamic> DeserializedRows { get; } = [];

    public Entry<dynamic> Deserialize(dynamic row)
    {
        DeserializeCallCount++;
        DeserializedRows.Add(row);
        
        // Return a simple entry for testing
        return EntryBuilder.CreateBlogPost($"entry{DeserializeCallCount}");
    }
}