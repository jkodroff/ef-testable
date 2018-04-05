using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkTestable.Testing
{
    public class FakeDbSet<T> : IDbSet<T>, IDbAsyncEnumerable<T> where T : class
    {
        private HashSet<T> _data;
        private IQueryable _query;
        private FakeDbAsyncEnumerator _asyncEnumerator;

        public FakeDbSet()
        {
            _data = new HashSet<T>();
            _query = _data.AsQueryable();
            _asyncEnumerator = new FakeDbAsyncEnumerator(_data.GetEnumerator());
        }

        public FakeDbSet(IEnumerable<T> data)
        {
            _data = new HashSet<T>(data);
            _query = _data.AsQueryable();
            _asyncEnumerator = new FakeDbAsyncEnumerator(_data.GetEnumerator());
        }

        public virtual T Find(params object[] keyValues)
        {
            // TODO: Figure out how to get this to work with multiple entitites in the
            // set.  We'll probably have to do some reflection and look for an Id property
            // to match on.
            if (_data.Count > 1)
                throw new Exception("Find only works with 1 or 0 entities in the collection.");
            return _data.Count == 0 ? null : _data.Single();
        }

        public T Add(T item)
        {
            _data.Add(item);
            return item;
        }

        public T Remove(T item)
        {
            _data.Remove(item);
            return item;
        }

        public T Attach(T item)
        {
            _data.Add(item);
            return item;
        }

        public T Create()
        {
            return null;
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return null;
        }

        public ObservableCollection<T> Local
        {
            get { return null; }
        }

        public void Detach(T item)
        {
            _data.Remove(item);
        }

        Type IQueryable.ElementType
        {
            get { return _query.ElementType; }
        }

        Expression IQueryable.Expression
        {
            get { return _query.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _query.Provider; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return _asyncEnumerator;
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        private class FakeDbAsyncEnumerator : IDbAsyncEnumerator<T>
        {
            private IEnumerator<T> _enumerator;

            public FakeDbAsyncEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public void Dispose() { }

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_enumerator.MoveNext());
            }

            public T Current => _enumerator.Current;

            object IDbAsyncEnumerator.Current => Current;
        }
    }
}