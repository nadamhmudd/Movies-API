using Movies.Core.Interfaces.Repositories;
using Movies.Core.Models;

namespace Movies.Core.Interfaces;
public interface IUnitOfWork
{
    //Register App Repositories
    public IBaseRepository<Genre> Genre { get; }
    public IMovieRepository Movie { get; }

    //Global Methods
    public void Save();
}
