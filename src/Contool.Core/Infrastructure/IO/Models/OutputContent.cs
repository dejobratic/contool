namespace Contool.Core.Infrastructure.IO.Models;

public class OutputContent
{
    public string FullPath { get; }

    public FileDataSource DataSource { get; }

    public IAsyncEnumerable<dynamic> Content { get; }

    public OutputContent(string path, string name, string type, IAsyncEnumerable<dynamic> content)
    {
        DataSource = (FileDataSource)Models.DataSource.From(type);
        FullPath = Path.Combine(path, $"{name}{DataSource.Extension}");
        Content = content;
    }
}