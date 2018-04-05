using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EntityFrameworkTestable
{
    public interface IDataSession : IDisposable
    {
        IDbSet<T> Set<T>() where T : class;
        void SaveChanges();
        Task SaveChangesAsync();
        int SqlCommand(string sql, params object[] parameters);
    }
}