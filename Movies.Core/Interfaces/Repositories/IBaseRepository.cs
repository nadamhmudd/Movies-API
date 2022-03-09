﻿using Movies.Service;
using System.Linq.Expressions;

namespace Movies.Core.Interfaces.Repositories;

//Generic Class
public interface IBaseRepository<T> where T : class
{
    //CRUD Operations
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>>? orderBy = null, string orderByDirection = SD.Ascending);
    T Update(T entity);
    void Delete(T entity);

    //Search operations
    Task<T> GetByIdAsync(int id);
    Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> criteria, bool tracked = false, string includeProperties = null);

    //Aggregating operations
    Task<int> CountAsync();
}