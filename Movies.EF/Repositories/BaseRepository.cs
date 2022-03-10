global using Microsoft.EntityFrameworkCore;
global using System.Linq.Expressions;
using Movies.Core.Interfaces;
using Movies.Core.Constants;

namespace Movies.EF.Repositories;
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
        Expression<Func<T, bool>>? criteria = null,
        string includeProperties = null,
        Expression<Func<T, object>>? orderBy = null,
        string orderByDirection = SD.Ascending)
    {
        IQueryable<T> query =_dbSet;

        if(criteria is not null)
            query = query.Where(criteria);

        if (includeProperties != null)
        {
            query = _IncludeProperties(query, includeProperties);
        }

        if (orderBy != null)
        {
            query = _OrderBy(query, orderBy, orderByDirection);
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

    public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> criteria, bool tracked = false, string includeProperties = null)
    {
        IQueryable<T> query = _Tracked(tracked, criteria);

        if (includeProperties != null)
        {
            query = _IncludeProperties(query, includeProperties);
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

    //-------------------Helper Method----------------------------------------
    private IQueryable<T> _IncludeProperties(IQueryable<T> query, string includeProperties)
    {
        foreach (var include in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(include);
        }

        return query;
    }

    private IQueryable<T> _OrderBy(IQueryable<T> query, Expression<Func<T, object>> orderBy, string orderByDirection)
    {
        if (orderByDirection == SD.Ascending)
            return query.OrderBy(orderBy);
        else
            return query.OrderByDescending(orderBy);
    }

    private IQueryable<T> _Tracked(bool tracked, Expression<Func<T, bool>> criteria)
    {
        if (tracked)
            return _dbSet.Where(criteria);
        else
            return _dbSet.AsNoTracking().Where(criteria);
    }
}
