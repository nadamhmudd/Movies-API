﻿using System.ComponentModel.DataAnnotations;

namespace Movies.API.DTOs
{
    //DTO -> Data transfer Object used with apis
    public class GenreDto
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}   