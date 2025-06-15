using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Input;

public class InputReaderFactory(IEnumerable<IInputReader> readers) : IInputReaderFactory
{
    private readonly Dictionary<DataSource, IInputReader> _readers =
        readers.ToDictionary(reader => reader.DataSource);

    public IInputReader Create(DataSource dataSource)
    {
        if (_readers.TryGetValue(dataSource, out var reader))
            return reader;

        // TODO: add more context
        throw new NotImplementedException();
    }
}