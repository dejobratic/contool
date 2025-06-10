using Contool.Infrastructure.IO.Models;

namespace Contool.Infrastructure.IO.Output;

internal class OutputWriterFactory(IEnumerable<IOutputWriter> writers) : IOutputWriterFactory
{
    private readonly Dictionary<DataSource, IOutputWriter> _writers = writers.ToDictionary(x => x.DataSource);

    public IOutputWriter Create(DataSource dataSource)
    {
        if (_writers.TryGetValue(dataSource, out var outputWriter))
        {
            return outputWriter;
        }

        throw new NotImplementedException();
    }
}
