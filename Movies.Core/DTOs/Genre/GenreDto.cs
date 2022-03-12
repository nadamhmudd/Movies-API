namespace Movies.Core.DTOs;

//DTO -> Data transfer Object used with apis
public class GenreDto
{
    [MaxLength(100)]
    public string Name { get; set; }
}
