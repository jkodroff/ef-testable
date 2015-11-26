using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Moq;

namespace EntityFrameworkTestable.Testing
{
    public static class DataHelper
    {
        public static IDataSession Session()
        {
            return new MockDataSession();
        }

        public static IReadOnlyDataSession ReadOnly(this IDataSession session)
        {
            var mockSession = session as MockDataSession;
            if (mockSession == null)
                throw new InvalidOperationException("'ReadOnly' can only be called on mock data sessions.");

            return mockSession;
        }

        public static IDataSession Session<T>(params T[] set) where T : class
        {
            return Session().AddSet(set);
        }

        public static IDataSession AddSet<T>(this IDataSession session, params T[] set) where T : class
        {
            var mockSession = session as MockDataSession;
            if (mockSession == null)
                throw new Exception(string.Format("Data store must of type MockDataStore.  Got type '{0}' instead", session.GetType()));

            mockSession
                .Mock
                .Setup(x => x.Set<T>())
                .Returns(set == null ? new FakeDbSet<T>() : new FakeDbSet<T>(set));

            return mockSession;
        }

        private class MockDataSession : IDataSession, IReadOnlyDataSession
        {
            private Mock<IDataSession> _mock = new Mock<IDataSession>();

            internal Mock<IDataSession> Mock
            {
                get { return _mock; }
            }

            public void Dispose() { }

            IDbSet<T> IDataSession.Set<T>()
            {
                return _mock.Object.Set<T>();
            }

            public IEnumerable<T> SqlQuery<T>(string sql, params object[] parameters)
            {
                throw new NotImplementedException();
            }

            public void SaveChanges() { }

            public Task SaveChangesAsync()
            {
                return Task.FromResult(false);
            }

            public int SqlCommand(string sql, params object[] parameters)
            {
                throw new NotImplementedException();
            }

            IQueryable<T> IReadOnlyDataSession.Set<T>()
            {
                return _mock.Object.Set<T>();
            }
        }
    }
}