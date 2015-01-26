using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace EntityFrameworkTestable
{
    public class ReadOnlyEntityDataSession : IReadOnlyDataSession
    {
        private readonly DbContext _context;

        public ReadOnlyEntityDataSession(DbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IQueryable<T> Set<T>() where T : class
        {
            return _context.Set<T>().AsNoTracking();
        }

        public IEnumerable<T> SqlQuery<T>(string sql, params object[] parameters)
        {
            return _context.Database.SqlQuery<T>(sql, parameters);
        }
    }
}