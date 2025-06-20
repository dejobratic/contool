﻿using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.IO.Services;

public class JsonInputReader : IInputReader
{
    public DataSource DataSource => DataSource.Json;

    public IAsyncEnumerableWithTotal<dynamic> ReadAsync(string path, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
