using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

public abstract class SysField(string name)
{
    public string Name { get; } = name;

    public static readonly SysField[] All =
    [
        new SysIdField(),
        new SysTypeField(),
        new SysContentTypeField(),
        new SysSpaceField(),
        new SysEnvironmentField(),
        new SysVersionField(),
        new SysPublishedVersionField(),
        new SysArchivedVersionField(),
        new SysCreatedAtField(),
        new SysUpdatedAtField(),
        new SysPublishedAtField(),
        new SysFirstPublishedAtField(),
        new SysArchivedAtField()
    ];

    public static string[] Names => [.. All.Select(f => f.Name)];

    public static void Serialize(SystemProperties sys, IDictionary<string, object?> output)
    {
        foreach (var field in All)
        {
            output[field.Name] = field.Extract(sys);
        }
    }

    public static void Deserialize(SystemProperties sys, string name, object? value)
    {
        var field = All.FirstOrDefault(f => f.Name == name);
        field?.Apply(sys, value);
    }

    public abstract object? Extract(SystemProperties sys);

    public abstract void Apply(SystemProperties sys, object? value);
}