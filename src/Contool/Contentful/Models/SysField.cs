using Contentful.Core.Models;
using Contool.Contentful.Utils;

namespace Contool.Contentful.Models;

public abstract class SysField(string name)
{
    public string Name { get; } = name;

    public abstract object? Extract(SystemProperties sys);

    public abstract void Apply(SystemProperties sys, object? value);

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
}

class SysIdField() : SysField("sys.Id")
{
    public override object? Extract(SystemProperties sys)
        => sys.Id;

    public override void Apply(SystemProperties sys, object? value)
    {
        sys.Id = value?.ToString();

        if (string.IsNullOrEmpty(sys.Id))
            sys.Id = ContentfulIdGenerator.NewId();
    }
}

class SysTypeField() : SysField("sys.Type")
{
    public override object? Extract(SystemProperties sys)
        => sys.Type;

    public override void Apply(SystemProperties sys, object? value)
        => sys.Type = value?.ToString();
}

class SysContentTypeField() : SysField("sys.ContentType")
{
    public override object? Extract(SystemProperties sys)
        => sys.ContentType?.SystemProperties.Id;

    public override void Apply(SystemProperties sys, object? value)
        => sys.ContentType.SystemProperties.Id = value?.ToString();
}

class SysSpaceField() : SysField("sys.Space")
{
    public override object? Extract(SystemProperties sys)
        => sys.Space?.SystemProperties.Id;

    public override void Apply(SystemProperties sys, object? value)
        => sys.Space.SystemProperties.Id = value?.ToString();
}

class SysEnvironmentField() : SysField("sys.Environment")
{
    public override object? Extract(SystemProperties sys)
        => sys.Environment?.SystemProperties.Id;

    public override void Apply(SystemProperties sys, object? value)
        => sys.Environment.SystemProperties.Id = value?.ToString();
}

class SysVersionField() : SysField("sys.Version")
{
    public override object? Extract(SystemProperties sys)
        => sys.Version;

    public override void Apply(SystemProperties sys, object? value)
    {
        _ = int.TryParse(value?.ToString(), out int v);
        sys.Version = v;
    }
}

class SysPublishedVersionField() : SysField("sys.PublishedVersion")
{
    public override object? Extract(SystemProperties sys)
        => sys.PublishedVersion;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (int.TryParse(value?.ToString(), out var v))
            sys.PublishedVersion = v;
    }
}

class SysArchivedVersionField() : SysField("sys.ArchivedVersion")
{
    public override object? Extract(SystemProperties sys)
        => sys.ArchivedVersion;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (int.TryParse(value?.ToString(), out var v))
            sys.ArchivedVersion = v;
    }
}

class SysCreatedAtField() : SysField("sys.CreatedAt")
{
    public override object? Extract(SystemProperties sys)
        => sys.CreatedAt;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.CreatedAt = v;
    }
}

class SysUpdatedAtField() : SysField("sys.UpdatedAt")
{
    public override object? Extract(SystemProperties sys)
        => sys.UpdatedAt;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.UpdatedAt = v;
    }
}

class SysPublishedAtField() : SysField("sys.PublishedAt")
{
    public override object? Extract(SystemProperties sys)
        => sys.PublishedAt;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.PublishedAt = v;
    }
}

class SysFirstPublishedAtField() : SysField("sys.FirstPublishedAt")
{
    public override object? Extract(SystemProperties sys)
        => sys.FirstPublishedAt;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.FirstPublishedAt = v;
    }
}

class SysArchivedAtField() : SysField("sys.ArchivedAt")
{
    public override object? Extract(SystemProperties sys)
        => sys.ArchivedAt;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.ArchivedAt = v;
    }
}
