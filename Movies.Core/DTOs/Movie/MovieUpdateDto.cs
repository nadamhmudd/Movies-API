namespace Movies.Core.DTOs;
public class MovieUpdateDto : BaseMovieDto
{
    public IFormFile? Poster { get; set; } //can update poster or not
}
