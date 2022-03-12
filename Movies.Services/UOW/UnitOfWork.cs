using Movies.Core.Entities.Models;
using Movies.Core.Interfaces;
using Movies.EF;
using Movies.EF.Repositories;

namespace Movies.Services.UOW;

public class UnitOfWork : IUnitOfWork
{
    //The only place can access database 
    private readonly ApplicationDbContext _db;
   
    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;

        //Initialize App Repositories
        Genre = new BaseRepository<Genre>(_db.Set<Genre>());
        Movie = new BaseRepository<Movie>(_db.Set<Movie>());
    }

    public IBaseRepository<Genre> Genre { get; private set; }
    public IBaseRepository<Movie> Movie { get; private set; }

    public void Save() => _db.SaveChanges();
}
