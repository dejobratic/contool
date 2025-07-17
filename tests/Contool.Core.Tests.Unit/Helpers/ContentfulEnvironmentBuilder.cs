using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Core.Tests.Unit.Helpers;

public class ContentfulEnvironmentBuilder
{
    private string _id = "master";

    public ContentfulEnvironmentBuilder WithId(string id)
    {
        _id = id;
        return this;
    }
    
    public ContentfulEnvironment Build()
    {
        return new ContentfulEnvironment
        {
            SystemProperties = new SystemProperties
            {
                Id = _id,
                Type = "Environment",
                Version = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
        };
    }

    public static ContentfulEnvironmentBuilder Create() => new();

    public static ContentfulEnvironment CreateDefault() => new ContentfulEnvironmentBuilder().Build();

    public static ContentfulEnvironment CreateMaster() => new ContentfulEnvironmentBuilder().Build();

    public static ContentfulEnvironment CreateStaging() => 
        new ContentfulEnvironmentBuilder()
            .WithId("staging")
            .Build();

    public static ContentfulEnvironment CreateProduction() => 
        new ContentfulEnvironmentBuilder()
            .WithId("production")
            .Build();

    public static List<ContentfulEnvironment> CreateMultiple() =>
    [
        CreateMaster(),
        CreateStaging(),
        CreateProduction()
    ];
}