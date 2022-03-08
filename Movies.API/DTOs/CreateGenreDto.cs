using System.ComponentModel.DataAnnotations;

namespace Movies.API.DTOs
{
    //DTO -> Data transfer Object used with apis
    public class CreateGenreDto
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}   
