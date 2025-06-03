using Contool.Models;

namespace Contool.Services;

internal interface IInputReaderFactory
{
    IInputReader Create(DataSource dataSource);
}