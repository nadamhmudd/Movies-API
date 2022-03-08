using Movies.Service;
using System.Linq.Expressions;

namespace Movies.Core.Interfaces.Repositories;

//Generic Class
public interface IBaseRepository<T> where T : class
{
    //CRUD Operations
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>>? orderBy = null, string orderByDirection = SD.Ascending);


    //Search Operations
   
    //Aggregating Operations
}
