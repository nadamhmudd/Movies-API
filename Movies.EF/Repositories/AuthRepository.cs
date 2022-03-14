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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly ITokenHandler _tokenHandler;

    public AuthRepository(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenHandler jwt, IMapper mapper)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._tokenHandler = jwt;
        _mapper = mapper;
    }

    public async Task<AuthDto> RegisterAsync(RegisterDto dto)
    {
        if(await _userManager.FindByEmailAsync(dto.Email) is not null)
            return new AuthDto { Message = "Email is already registered!" };

        if (await _userManager.FindByNameAsync(dto.UserName) is not null)
            return new AuthDto { Message = "Username is already registered!" };

        var user = _mapper.Map<ApplicationUser>(dto);
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = "";
            foreach (var error in result.Errors)
                errors += $"{error.Description}, ";
            errors = errors.Trim(','); //remove last ,

            return new AuthDto { Message = errors };
        }

        await _userManager.AddToRoleAsync(user, SD.Role_User); //by default

        //create JWT
        var jwtSecurityToken = await _tokenHandler.CreateJwtToken(user);

        return new AuthDto
        {
            Email = user.Email,
            UserName = user.UserName,
            Roles = new List<string> { SD.Role_User },
            IsAuthenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            //ExpiresOn = jwtSecurityToken.ValidTo
        };
    }

    public async Task<AuthDto> GetTokenAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        
        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return new AuthDto { Message = "Email or Password is incorrect!" };

        var jwtSecurityToken = await _tokenHandler.CreateJwtToken(user);
        
        var refreshToken = await _tokenHandler.CreateRefreshToken(user);

        return new AuthDto
        {
            Email = user.Email,
            UserName = user.UserName,
            Roles = (await _userManager.GetRolesAsync(user)).ToList(),
            IsAuthenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresOn,
        };
    }

    public async Task<string> AddRoleAsync(AddRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if (user is null || !await _roleManager.RoleExistsAsync(dto.Role))
            return "Invalid user ID or Role";

        if(await _userManager.IsInRoleAsync(user, dto.Role))
            return "User already assigned to this role";

        var result = await _userManager.AddToRoleAsync(user, dto.Role);

        return result.Succeeded ? String.Empty : "Something went wrong";
    }

    public async Task<AuthDto> RefreshTokenAsync(string token)
    {
        var authDto = new AuthDto();

        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if(user is null)
        {
            authDto.Message = "Invalid Token";
            return authDto;
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
        if (!refreshToken.IsActive)
        {
            authDto.Message = "Inactive Token";
            return authDto;
        }

        //step 1 : revoke refreshToken
        refreshToken.RevokedOn = DateTime.UtcNow;

        //step 2 : create new refreshToken
        var newRefreshToken = await _tokenHandler.CreateRefreshToken(user);

        //step 3 : Create JWT Token 
        var jwtToken = await _tokenHandler.CreateJwtToken(user);


        authDto.Email = user.Email;
        authDto.UserName = user.UserName;
        authDto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
        authDto.IsAuthenticated = true;
        authDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        authDto.RefreshToken = newRefreshToken.Token;
        authDto.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

        return authDto;
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
        
        if (user is null)
            return false;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
        
        if (!refreshToken.IsActive)
            return false;

        refreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return true;
    }
}
