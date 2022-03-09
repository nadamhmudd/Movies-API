using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Movies.Core.Models;
public class Movie
{
    public int Id { get; set; }
    
    [MaxLength(250)]
    public string Title { get; set; }

    [RegularExpression(@"\d{4}")]
    public int Year { get; set; }

    public double Rate { get; set; }

    [MaxLength(2500)]
    public string StoryLine { get; set; }

    public string PosterUrl { get; set; }

    public byte GenreId { get; set; }
    [ValidateNever]
    public Genre Genre { get; set; }

}
