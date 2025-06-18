using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Services;

public interface IOutputWriterFactory
{
    IOutputWriter Create(DataSource dataSource);
}