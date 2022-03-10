namespace Movies.Services.UOW;
public interface IUnitOfWork
{
    //Register App Repositories
    IBaseRepository<Genre> Genre { get; }
    IBaseRepository<Movie> Movie { get; }

    //Global Methods
    void Save();
}
