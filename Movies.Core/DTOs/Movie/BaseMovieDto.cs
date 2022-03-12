global using Microsoft.AspNetCore.Http;

namespace Movies.Core.DTOs;
public class BaseMovieDto 
{
    [MaxLength(250)]
    public string Title { get; set; }

    [RegularExpression(@"\d{4}")]
    public int Year { get; set; }

    [Range(0,10)]
    public double Rate { get; set; }

    [MaxLength(2500)]
    public string StoryLine { get; set; }

    public int GenreId { get; set; }
}
