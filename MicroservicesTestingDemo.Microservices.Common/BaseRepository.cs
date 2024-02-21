using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace MicroservicesTestingDemo.Microservices.Common
{
    public abstract class BaseRepository<DbType, T>(DbType context, IBaseUnitOfWork unitOfWork): IBaseRepository<T> where T : BaseModel where DbType : DbContext
    {
        protected readonly DbType _context = context;
        private readonly IBaseUnitOfWork unitOfWork = unitOfWork;
        protected readonly ILogger<BaseRepository<DbType, T>> _logger = unitOfWork.GetLogger<BaseRepository<DbType, T>>();
        protected readonly IMapper _mapper = unitOfWork.Mapper;

        protected string[] Includes { get; set; } = [];
        public Expression<Func<T, object>>? OrderByProperty { get; set; }

        public DbSet<T> GetTable() => _context.Set<T>();
        public IQueryable<T> GetQueryable() => GetTable().AsQueryable();

        public async Task<bool> Delete(T entity, CancellationToken cancellationToken = default)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var model = await Get(id);
            _context.Remove(model);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public IQueryable<T> Filter(Expression<Func<T, bool>> predicate, GlobalFilters<T>? filters = null)
        {
            Expression<Func<T, bool>> expression = (T x) => !x.IsDeleted;
            predicate = expression.CombineWithAndAlso(predicate);
            var response = GetQueryable().Where(predicate);
            if (filters != null)
            {
                var properties = typeof(T).GetProperties().Where(i => filters.Any(o => i.Name.Contains(o.Property))).ToList();
                foreach (var filter in filters)
                {
                    if (filter != null && filter.Expression != null)
                    {
                        _logger.LogInformation($"Filtering <{typeof(T).Name}>: {filter.Property} {filter.Filter} {filter.Value}");
                        response = response.Where(filter.Expression);
                    }
                }
            }
            if (OrderByProperty != null)
            {
                response = response.OrderBy(OrderByProperty);
            }
            return response;
        }

        public async Task<T?> FindAsync(Guid pk, CancellationToken cancellationToken)
        {
            return await Filter(i => i.Id == pk).FirstOrDefaultAsync(cancellationToken);
        }

        public IQueryable<T> GetAllAsync()
        {
            return GetTable();
        }

        public virtual async Task<T> Get(Guid id, Func<IQueryable<T>, IQueryable<T>>? condition = null)
        {
            IQueryable<T> queryable = _context.Set<T>();
            if (condition is not null)
            {
                queryable = condition(queryable);
            }
            if (Includes != null && Includes.Length != 0)
            {
                foreach (var include in Includes)
                {
                    queryable = queryable.Include(include);
                }
            }
            var entity = await queryable.FirstOrDefaultAsync(x => x.Id == id);
            return entity ?? throw new KeyNotFoundException($"Unable to find the key [{id}] for model [{nameof(T)}]");
        }

        public async Task<T> Insert(T entity, CancellationToken cancellationToken = default)
        {
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            var result = await _context.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        public async Task LoadRelation<TProperty>(T model, Expression<Func<T, TProperty?>> expression) where TProperty : class
        {
            await _context.Entry(model).Reference(expression).LoadAsync();
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // TODO
                _logger.LogError(ex, $"Unable to save changes for model [{nameof(T)}]");
                return false;
            }
            return true;
        }

        public async Task<T> Update(T entity, CancellationToken cancellationToken = default)
        {
            var result = _context.Update(entity);
            entity.UpdatedAt = DateTime.Now;
            if (entity.CreatedAt == DateTime.MinValue)
            {
                entity.CreatedAt = entity.UpdatedAt;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetQueryable().GetEnumerator();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await Filter(predicate).AnyAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetTable().CountAsync(predicate);
        }

        public IQueryable<T> IncludeFilter(Expression<Func<T, bool>> predicate)
        {
            var k = Filter(predicate);
            if (Includes != null && Includes.Any())
            {
                foreach (var include in Includes)
                {
                    k = k.Include(include);
                }
            }
            return k;
        }


        public void SetOrderBy(Expression<Func<T, object>> orderBy)
        {
            OrderByProperty = orderBy;
        }


        public async Task DeleteRange(ICollection<T> workShifts, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().RemoveRange(workShifts);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> InsertRange(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            await _context.Set<T>().AddRangeAsync(entities, cancellationToken);
            var r = await _context.SaveChangesAsync(cancellationToken);
            if (r != entities.Count())
            {
                _logger.LogError("Error registering requests");
            }
            return entities;
        }
    }

}
