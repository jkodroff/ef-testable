using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityFrameworkTestable
{
    public interface IReadOnlyDataSession : IDisposable
    {
        IQueryable<T> Set<T>() where T : class;
        IEnumerable<T> SqlQuery<T>(string sql, params object[] parameters);
    }
}