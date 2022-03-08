using Microsoft.EntityFrameworkCore;
using Movies.Core.Interfaces.Repositories;
using Movies.Service;
using System.Linq.Expressions;

namespace Movies.DataAccess.Repositories;
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected DbSet<T> _dbSet;
    public BaseRepository(DbSet<T> dbSet) => _dbSet = dbSet;


    //CRUD opertaions
    public void Add(T entity) => _dbSet.Add(entity);

    public void AddRange(IEnumerable<T> entities) => _dbSet.AddRange(entities);

     //_db.ShoppingCarts.Include(u => u.Product).Include(u=>u.CoverType);
    //includeProp - "Category,CoverType"
    public IEnumerable<T> GetAll(string includeProperties = null,
        Expression<Func<T, object>>? orderBy = null, string orderByDirection = SD.Ascending,
        Expression<Func<T, bool>>? criteria = null)
    {
        IQueryable<T> query = _dbSet;

        if(criteria != null)
        {
            query = query.Where(criteria);
        }
        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        
        if (orderBy != null)
            if (orderByDirection == SD.Ascending)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderByDescending(orderBy);

        return query.ToList();
    }

    public void Update(T entity) => _dbSet.Update(entity);

    public void Remove(T entity) => _dbSet.Remove(entity);

    public  void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);


    //Search operations
    public T GetById(int id) => _dbSet.Find(id);

    public T GetFirstOrDefault(Expression<Func<T, bool>> criteria, string includeProperties = null, bool tracked = true)
    {
        IQueryable<T> query;
        if (tracked)
        {
           query = _dbSet.Where(criteria);
        }
        else
        {
            query = _dbSet.AsNoTracking().Where(criteria);
        }

        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return query.FirstOrDefault();
    }

    public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string includeProperties = null,
                                  Expression<Func<T, object>>? orderBy = null,
                                  string orderByDirection = SD.Ascending)
    {
        IQueryable<T> query = _dbSet.Where(criteria);

        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        if (orderBy != null)
            if (orderByDirection == SD.Ascending)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderByDescending(orderBy);

        return query.ToList();

    }

    //Aggregating operations
    public int Count() => _dbSet.Count();
}
