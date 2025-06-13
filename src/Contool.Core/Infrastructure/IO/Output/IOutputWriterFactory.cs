using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Output;

public interface IOutputWriterFactory
{
    IOutputWriter Create(DataSource dataSource);
}