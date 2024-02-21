using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MicroservicesTestingDemo.Microservices.Common
{
    public interface IBaseRepository<T> where T : BaseModel
    {
        Task<IEnumerable<T>> InsertRange(IEnumerable<T> entities, CancellationToken cancellationToken);
        Expression<Func<T, object>>? OrderByProperty { get; set; }
        DbSet<T> GetTable();
        IQueryable<T> GetQueryable();
        Task<T?> FindAsync(Guid pk, CancellationToken cancellationToken);
        Task<bool> Delete(T entity, CancellationToken cancellationToken);
        Task<bool> Delete(Guid id, CancellationToken cancellationToken);
        IQueryable<T> Filter(Expression<Func<T, bool>> predicate, GlobalFilters<T>? filters = null);
        IQueryable<T> GetAllAsync();
        Task<T> Get(Guid id, Func<IQueryable<T>, IQueryable<T>>? condition = null);
        Task<T> Insert(T entity, CancellationToken cancellationToken);
        Task LoadRelation<TProperty>(T model, Expression<Func<T, TProperty?>> expression) where TProperty : class;
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
        Task<T> Update(T entity, CancellationToken cancellationToken);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        public IQueryable<T> IncludeFilter(Expression<Func<T, bool>> predicate);
        void SetOrderBy(Expression<Func<T, object>> orderBy);
        Task DeleteRange(ICollection<T> workShifts, CancellationToken cancellationToken);
    }

}
