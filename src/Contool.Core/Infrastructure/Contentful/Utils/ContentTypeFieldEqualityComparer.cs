using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contool.Core.Infrastructure.Contentful.Utils;

public class ContentTypeFieldEqualityComparer : IEqualityComparer<Field>
{
    public bool Equals(Field? x, Field? y)
    {
        throw new NotImplementedException();
    }

    public int GetHashCode([DisallowNull] Field obj)
    {
        throw new NotImplementedException();
    }
}
