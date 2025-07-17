using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Core.Tests.Unit.Helpers;

public class SpaceBuilder
{
    private string _id = "test-space";
    private string _name = "Test Space";

    public SpaceBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public SpaceBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public Space Build()
    {
        return new Space
        {
            SystemProperties = new SystemProperties
            {
                Id = _id,
                Type = "Space",
                Version = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            Name = _name,
        };
    }

    public static SpaceBuilder Create() => new();

    public static Space CreateDefault() => new SpaceBuilder().Build();

    public static Space CreateWithId(string id) => new SpaceBuilder().WithId(id).Build();

    public static Space CreateWithName(string name) => new SpaceBuilder().WithName(name).Build();

    public static List<Space> CreateMultiple() =>
    [
        CreateWithId("space-1"),
        CreateWithId("space-2"),
        CreateWithId("space-3"),
    ];
}