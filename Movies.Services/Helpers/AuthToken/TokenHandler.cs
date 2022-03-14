using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Core.appsettings;
using Movies.Core.Constants;
using Movies.Core.Entities.Models;
using Movies.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace Movies.Services.Helpers;
public class TokenHandler : ITokenHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JWT _jwt;

    public TokenHandler(UserManager<ApplicationUser> userManager, 
        IOptions<JWT> jwt)
    {
        _userManager = userManager;
        _jwt = jwt.Value;
    }

    public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles      = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();
        foreach (var role in roles)
            roleClaims.Add(new Claim("roles", role));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(SD.UserId, user.Id)
    }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer:   _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddDays(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials
            );

        return jwtSecurityToken;
    }

    public async Task<RefreshToken> CreateRefreshToken(ApplicationUser user)
    {
        var refreshToken = new RefreshToken();

        if (user.RefreshTokens.Any(t => t.IsActive))
            refreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
        else
        {
            refreshToken = _generateRefreshToken();

            //add to db
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
        }

        return refreshToken;
    }

    //---------------Helper Method--------------------------
    private RefreshToken _generateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var generator = new RNGCryptoServiceProvider();

        generator.GetBytes(randomNumber);

        return new RefreshToken { Token = Convert.ToBase64String(randomNumber) };
    }
}
