using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Services;

public interface IInputReaderFactory
{
    IInputReader Create(DataSource dataSource);
}