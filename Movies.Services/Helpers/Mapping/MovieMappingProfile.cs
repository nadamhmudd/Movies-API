using AutoMapper;
using Movies.Core.Entities.Models;
using Movies.Core.DTOs;
using Movies.Services.Extensions;

namespace Movies.Services.Helpers.Mapping;
public class MovieMappingProfile : Profile
{
    public MovieMappingProfile()
    {
        CreateMap<Movie, MovieDetailsDto>()
            .ForMember(dest => dest.GenerName, opt => opt.MapFrom(
                src => src.Genre.Name));

        CreateMap<BaseMovieDto, Movie>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(
                src => src.Title.Trim().CapitalizeFistLitter()))
            .ForMember(dest => dest.PosterUrl, opt => opt.Ignore());


    }
}
