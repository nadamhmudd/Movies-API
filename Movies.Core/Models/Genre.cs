using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Core.Models;
public class Genre
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public byte Id { get; set; } //max 255

    [MaxLength(100)]
    public string Name { get; set; } //by default all props required no need to [Required]
}
