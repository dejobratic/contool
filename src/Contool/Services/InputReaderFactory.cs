using Contool.Models;

namespace Contool.Services;

internal class InputReaderFactory(IEnumerable<IInputReader> readers) : IInputReaderFactory
{
    private readonly Dictionary<DataSource, IInputReader> _readers = readers.ToDictionary(reader => reader.DataSource);

    public IInputReader Create(DataSource dataSource)
    {
        if (_readers.TryGetValue(dataSource, out var reader))
        {
            return reader;
        }

        throw new NotImplementedException();
    }
}