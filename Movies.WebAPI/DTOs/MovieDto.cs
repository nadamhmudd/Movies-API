namespace Movies.Movies.WebAPI.DTOs;
public class MovieDto 
{
    [MaxLength(250)]
    public string Title { get; set; }

    [RegularExpression(@"\d{4}")]
    public int Year { get; set; }

    [Range(0,10)]
    public double Rate { get; set; }

    [MaxLength(2500)]
    public string StoryLine { get; set; }

    public IFormFile? Poster { get; set; }

    public int GenreId { get; set; }
}
