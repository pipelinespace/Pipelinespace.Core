using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        void Add(TEntity obj);
        void Update(TEntity obj);
        void Remove(Guid id);
        Task<List<TEntity>> Find(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);
        Task<TEntity> FindFirst(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);
        Task<List<TEntity>> GetAll();
        Task<int> SaveChanges();
    }
}
