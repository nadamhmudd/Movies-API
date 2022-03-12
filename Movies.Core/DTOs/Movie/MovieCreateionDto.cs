namespace Movies.Core.DTOs;
public class MovieCreateionDto : BaseMovieDto
{
    public IFormFile Poster { get; set; } //poster can't be null
}
