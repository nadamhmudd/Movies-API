using Microsoft.EntityFrameworkCore;
using Movies.Core.Interfaces;
using Movies.Core.Models;

namespace Movies.DataAccess.Repositories;
public class MovieRepository : BaseRepository<Movie>, IMovieRepository
{
    public MovieRepository(DbSet<Movie> dbSet) : base(dbSet)
    {
    }

    //override
    public Movie Update(Movie entity)
    {
        if (entity.PosterUrl is null)
            entity.PosterUrl = _dbSet.Find(entity.Id).PosterUrl;

        _dbSet.Update(entity);
        
        return entity;
    }
}
