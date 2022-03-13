using Movies.Core.Entities.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Movies.Core.Interfaces;
public interface IJWT
{
    Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
}
