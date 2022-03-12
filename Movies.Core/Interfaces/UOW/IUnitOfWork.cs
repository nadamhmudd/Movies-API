using Movies.Core.appsettings;
using Movies.Core.Entities.Models;

namespace Movies.Core.Interfaces;
public interface IUnitOfWork
{
    //Register App Repositories
    IBaseRepository<Genre> Genre { get; }
    IBaseRepository<Movie> Movie { get; }
    IAuthRepository Auth { get;  }

    //Global Methods
    void Save();
}
