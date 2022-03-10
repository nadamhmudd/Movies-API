namespace Movies.Movies.WebAPI.DTOs;
public class MovieCreationDto : BaseMovieDto
{
    public IFormFile Poster { get; set; } //poster can't be null
}
