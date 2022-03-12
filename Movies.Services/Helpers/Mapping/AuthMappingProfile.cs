using AutoMapper;
using Movies.Core.DTOs;
using Movies.Core.Entities.Models;

namespace Movies.Services.Helpers.Mapping;
public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<RegisterDto, ApplicationUser>();
    }
}
