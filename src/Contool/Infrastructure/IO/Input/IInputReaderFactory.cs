using Contool.Infrastructure.IO.Models;

namespace Contool.Infrastructure.IO.Input;

internal interface IInputReaderFactory
{
    IInputReader Create(DataSource dataSource);
}