namespace Contool.Infrastructure.IO.Models;

public class FileDataSource(string name, string extension) : DataSource(name)
{
    public string Extension { get; } = extension;

    public override string ToString() => $"{Name} ({Extension})";
}
