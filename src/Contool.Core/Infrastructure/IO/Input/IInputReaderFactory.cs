using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Input;

public interface IInputReaderFactory
{
    IInputReader Create(DataSource dataSource);
}