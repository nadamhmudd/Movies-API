using Movies.Core.DTOs;

namespace Movies.Core.Interfaces;
public interface IAuthRepository
{
    Task<AuthDto> RegisterAsync(RegisterDto dto);
    Task<AuthDto> GetTokenAsync(TokenRequestDto dto);
    Task<string> AddRoleAsync(AddRoleDto dto);
}
