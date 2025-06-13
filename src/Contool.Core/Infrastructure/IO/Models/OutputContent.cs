namespace Contool.Core.Infrastructure.IO.Models;

public class OutputContent : Content
{
    public string FullPath { get; }

    public FileDataSource DataSource { get; }

    public OutputContent(string path, string name, string type)
    {
        DataSource = (FileDataSource)Models.DataSource.From(type);
        FullPath = Path.Combine(path, $"{name}{DataSource.Extension}");
    }
}