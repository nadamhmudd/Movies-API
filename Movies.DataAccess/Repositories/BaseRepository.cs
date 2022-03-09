using Microsoft.EntityFrameworkCore;
using Movies.Core.Interfaces.Repositories;
using Movies.Service;
using System.Linq.Expressions;

namespace Movies.DataAccess.Repositories;
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected DbSet<T> _dbSet;
    public BaseRepository(DbSet<T> dbSet)
    {
        _dbSet = dbSet;
    }

    //CRUD opertaions
    public async Task<T> AddAsync(T entity)
    {
        _dbSet.Add(entity);
        return entity;
    }
    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, object>>? orderBy = null,
        string orderByDirection = SD.Ascending)
    {
        IQueryable<T> query =_dbSet;

        if(orderBy != null)
        {
            if(orderByDirection == SD.Ascending)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderByDescending(orderBy);
        }

        return await query.ToListAsync();
    }
    public T Update(T entity)
    {
        _dbSet.Update(entity);
        return entity;
    }
    public void Delete(T entity) => _dbSet.Remove(entity);

    //Search operations
    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> criteria,
        bool tracked = false,
        string includeProperties = null)
    {
        IQueryable<T> query;
        if (tracked)
            query = _dbSet.Where(criteria);
        else
            query = _dbSet.AsNoTracking().Where(criteria);

        if (includeProperties != null)
        {
            foreach (var include in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(include);
            }
        }

        return await query.FirstOrDefaultAsync(criteria);
    }

    //Aggregating operations
    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<bool> IsValidAsync(Expression<Func<T, bool>> criteria)
    {
        return await _dbSet.AnyAsync(criteria);
    }

}
