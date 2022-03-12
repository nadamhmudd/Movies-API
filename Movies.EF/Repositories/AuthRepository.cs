using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Core.appsettings;
using Movies.Core.Constants;
using Movies.Core.DTOs;
using Movies.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Movies.EF.Repositories;
public class AuthRepository : IAuthRepository
{
    private UserManager<ApplicationUser> _userManager;
    private RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    private JWT _jwt;

    public AuthRepository(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<JWT> jwt, IMapper mapper)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._jwt = jwt.Value;
        _mapper = mapper;
    }

    public async Task<AuthDto> RegisterAsync(RegisterDto dto)
    {
        if(await _userManager.FindByEmailAsync(dto.Email) is not null)
            return new AuthDto { Message = "Email is already registered!" };

        if(await _userManager.FindByNameAsync(dto.UserName) is not null)
            return new AuthDto { Message = "Username is already registered!" };

        var user = _mapper.Map<ApplicationUser>(dto);
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = "";
            foreach (var error in result.Errors)
                errors += $"{error.Description}, ";
            errors += errors.Trim(','); //remove last ,

            return new AuthDto { Message = errors };
        }

        await _userManager.AddToRoleAsync(user, SD.Role_User); //by default

        //vreate JWT
        var jwtSecurityToken = await CreateJwtToken(user);

        return new AuthDto
        {
            Email = user.Email,
            UserName = user.UserName,
            Roles = new List<string> { SD.Role_User },
            IsAuthenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            ExpiresOn = jwtSecurityToken.ValidTo
        };
    }

    //------------ Helper Method--------------------------------
    private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

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
        var signingCredentials   = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddDays(_jwt.DurationInDays),
            signingCredentials: signingCredentials
            );

        return jwtSecurityToken;
    }
}
