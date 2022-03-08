using Movies.Service;
using System.Linq.Expressions;

namespace Movies.Core.Interfaces.Repositories;

//Generic Class
public interface IBaseRepository<T> where T : class
{
    //CRUD Operations
    public void Add(T entity);
    public void AddRange(IEnumerable<T> entities);
    public IEnumerable<T> GetAll(string? includeProperties = null, Expression<Func<T, object>>? orderBy = null, string orderByDirection = SD.Ascending, Expression<Func<T, bool>>? criteria=null);
    public void Update(T entity);
    public void Remove(T entity);
    public void RemoveRange(IEnumerable<T> entities);

    //Search Operations
    public T GetById(int id);
    public T GetFirstOrDefault(Expression<Func<T, bool>> criteria, string includeProperties = null, bool tracked = true);
    public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string includeProperties = null,
                                  Expression<Func<T, object>>? orderBy = null,
                                  string orderByDirection = SD.Ascending);

    //Aggregating Operations
    public int Count();
}
