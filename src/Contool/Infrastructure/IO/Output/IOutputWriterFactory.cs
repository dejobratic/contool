using Contool.Infrastructure.IO.Models;

namespace Contool.Infrastructure.IO.Output;

internal interface IOutputWriterFactory
{
    IOutputWriter Create(DataSource dataSource);
}