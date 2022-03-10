namespace Movies.Core.Entities.Models;
public class Movie : BaseEntity
{
    [MaxLength(250)]
    public string Title { get; set; }

    [RegularExpression(@"\d{4}")]
    public int Year { get; set; }

    public double Rate { get; set; }

    [MaxLength(2500)]
    public string StoryLine { get; set; }

    public string PosterUrl { get; set; }

    public int GenreId { get; set; }
    public Genre Genre { get; set; }
}
