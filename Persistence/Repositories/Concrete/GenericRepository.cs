using BambooExchangeRateService.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Expressions;
using System;

namespace BambooExchangeRateService.Persistence.Repositories.Concrete
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly ExchangeRateDbContext DbContext;

        public GenericRepository(ExchangeRateDbContext dBContext)
        {
            DbContext = dBContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                await DbContext.Set<T>().AddAsync(entity);
                return entity;
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString(), $"Error occured while adding {typeof(T).Name} to database");
                throw new Exception($"Error occured while adding", ex);
            }
        }
        public async Task UpdateRange(List<T> entity)
        {
            try
            {
                DbContext.Set<T>().UpdateRange(entity);
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString(), $"Error occured while adding {typeof(T).Name} to database");
                throw new Exception($"Error occured while adding", ex);
            }
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            try
            {
                return await DbContext.Database.BeginTransactionAsync();
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException($"Error occured while getting {typeof(T).Name} to database", ex);
            }
        }
        public async Task<T> DeleteAsync(int id)
        {
            try
            {
                var entity = await DbContext.Set<T>().FindAsync(id);

                DbContext.Set<T>().Remove(entity);

                return entity;
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException($"Error occured while deleting {typeof(T).Name} from database", ex);
            }
        }
        public async Task<IEnumerable<T>> Get(System.Linq.Expressions.Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            try
            {
                IQueryable<T> query = DbContext.Set<T>();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (orderBy != null)
                {
                    return await orderBy(query).ToListAsync();
                }
                else
                {
                    return await query.ToListAsync();
                }
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException($"Error occured while getting {typeof(T).Name} from database", ex);
            }
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            try
            {
                return await DbContext.Set<T>().ToListAsync();
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException($"Error occured while getting {typeof(T).Name} to database", ex);
            }
        }
        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                return await DbContext.Set<T>().FindAsync(id);
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException($"Error occured while getting {typeof(T).Name} to database", ex);
            }
        }
        public IQueryable<T> GetIQueryable()
        {
            try
            {
                return DbContext.Set<T>();
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException($"Error occured while getting {typeof(T).Name} to database", ex);
            }
        }
        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                DbContext.Set<T>().Update(entity);

                return await Task.FromResult(entity);
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException($"Error occured while updating {typeof(T).Name} to database", ex);
            }
        }
        public async Task AddRangeAsync(List<T> entity)
        {
            try
            {
                await DbContext.Set<T>().AddRangeAsync(entity);
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString(), $"Error occured while adding {typeof(T).Name} to database");
                throw new Exception($"Error occured while adding", ex);
            }
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await DbContext.Set<T>().AnyAsync(predicate);
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException($"Error occured while getting {typeof(T).Name} to database", ex);
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                 await DbContext.SaveChangesAsync();
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException($"Error occured while getting {typeof(T).Name} to database", ex);
            }
        }
    }
}
