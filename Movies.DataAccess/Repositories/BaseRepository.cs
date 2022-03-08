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

    //Search operations



    //Aggregating operations
}
