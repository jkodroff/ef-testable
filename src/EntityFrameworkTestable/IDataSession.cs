using System;
using System.Data.Entity;

namespace EntityFrameworkTestable
{
    public interface IDataSession : IDisposable
    {
        IDbSet<T> Set<T>() where T : class;
        void SaveChanges();
        int SqlCommand(string sql, params object[] parameters);
    }
}