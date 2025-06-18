using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Services;

public class OutputWriterFactory(IEnumerable<IOutputWriter> writers) : IOutputWriterFactory
{
    private readonly Dictionary<DataSource, IOutputWriter> _writers =
        writers.ToDictionary(x => x.DataSource);

    public IOutputWriter Create(DataSource dataSource)
    {
        if (_writers.TryGetValue(dataSource, out var outputWriter))
            return outputWriter;

        throw new NotImplementedException();
    }
}
