using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Infrastructure.Core;
using TransactionalBehavior = Infrastructure.Core.TransactionalBehavior;

namespace UnitTest
{
    internal sealed class MockDbSet<T> : DbSet<T>, IQueryable, IEnumerable<T> where T : class
    {
        readonly ObservableCollection<T> _data;
        readonly IQueryable _query;

        public MockDbSet()
        {
            _data = new ObservableCollection<T>();
            _query = _data.AsQueryable();
        }

        public override T Add(T entity)
        {
            _data.Add(entity);
            return entity;
        }

        public override T Attach(T entity)
        {
            _data.Add(entity);
            return entity;
        }

        public override T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override ObservableCollection<T> Local
        {
            get { return new ObservableCollection<T>(_data); }
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
    }

    public class MockContent : DbContext, IUnitOfWork
    {
        public MockContent()
            : base("CAAdmin")
        {

        }

        public override DbSet<TEntity> Set<TEntity>()
        {
            return new MockDbSet<TEntity>();
        }

        //public override D

        public override int SaveChanges()
        {
            return 1;
        }

        public bool TransactionEnabled { get; set; }
        public int ExecuteSqlCommand(TransactionalBehavior transactionalBehavior, string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable SqlQuery(Type elementType, string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteSqlCommandAsync(TransactionalBehavior transactionalBehavior, string sql, params object[] paramters)
        {
            throw new NotImplementedException();
        }
    }
}
