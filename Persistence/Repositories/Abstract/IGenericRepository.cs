using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace BambooExchangeRateService.Persistence.Repositories.Abstract
{
    public interface IGenericRepository<T>
    {
        IQueryable<T> GetIQueryable();
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IEnumerable<T>> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
        Task<T> GetByIdAsync(int id);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(List<T> entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(int id);
        Task SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
