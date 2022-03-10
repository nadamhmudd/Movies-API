global using Movies.Entities.Models;
global using Movies.Repo;

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

    void IUnitOfWork.Save()
    {
        throw new NotImplementedException();
    }
}
