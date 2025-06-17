using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Infrastructure.IO.Models;

public class OutputContent
{
    public string FullPath { get; }

    public FileDataSource DataSource { get; }

    public IAsyncEnumerableWithTotal<dynamic> Content { get; }

    public OutputContent(string path, string name, string type, IAsyncEnumerableWithTotal<dynamic> content)
    {
        DataSource = (FileDataSource)Models.DataSource.From(type);
        FullPath = Path.Combine(path, $"{name}{DataSource.Extension}");
        Content = content;
    }
}