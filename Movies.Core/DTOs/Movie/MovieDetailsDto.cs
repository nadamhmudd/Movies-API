namespace Movies.Core.DTOs;
public class MovieDetailsDto : BaseMovieDto
{
    public int Id { get; set; }
    public string PosterUrl{ get; set; }
    public string GenerName { get; set; }
}
