namespace Movies.Movies.Core.DTOs;
public class MovieDetailsDto : BaseMovieDto
{
    public IFormFile Poster { get; set; } //poster can't be null
}
