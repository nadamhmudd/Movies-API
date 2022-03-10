namespace Movies.Core.Entities.Models;
public class Genre : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } //by default all props required no need to [Required]
}
