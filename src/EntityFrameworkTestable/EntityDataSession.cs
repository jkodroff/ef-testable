using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EntityFrameworkTestable
{
    public class EntityDataSession : IDataSession
    {
        private readonly DbContext _dbContext;

        public EntityDataSession(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");

            _dbContext = dbContext;
        }

        public void Dispose()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
        }

        public IDbSet<T> Set<T>() where T : class
        {
            return _dbContext.Set<T>();
        }


        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public Task SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public int SqlCommand(string sql, params object[] parameters)
        {
            return _dbContext.Database.ExecuteSqlCommand(sql, parameters);
        }
    }
}