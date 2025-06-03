using Contool.Models;

namespace Contool.Services;

internal interface IOutputWriterFactory
{
    IOutputWriter Create(DataSource dataSource);
}