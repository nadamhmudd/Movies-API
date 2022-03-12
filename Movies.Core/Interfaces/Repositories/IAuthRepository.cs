using Movies.Core.DTOs;

namespace Movies.Core.Interfaces;
public interface IAuthRepository
{
    Task<AuthDto> RegisterAsync(RegisterDto dto);
}
