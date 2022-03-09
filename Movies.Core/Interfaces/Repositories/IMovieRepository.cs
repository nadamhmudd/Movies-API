using Movies.Core.Interfaces.Repositories;
using Movies.Core.Models;

namespace Movies.Core.Interfaces;

public interface IMovieRepository : IBaseRepository<Movie>
{
    Movie Update(Movie entity);
}
