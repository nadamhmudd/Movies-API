using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Movies.Core.Constants;
using Movies.Core.DTOs;
using Movies.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Movies.EF.Repositories;
public class AuthRepository : IAuthRepository
{
    #region Props
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly ITokenHandler _tokenHandler;
    #endregion

    #region Constructor(s)
    public AuthRepository(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenHandler jwt, IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenHandler = jwt;
        _mapper = mapper;
    }
    #endregion

    #region Actions
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

        //create Tokens
        var jwtSecurityToken = await _tokenHandler.CreateJwtToken(user);

        var refreshToken = await _tokenHandler.CreateRefreshToken(user);

        return _mapAuthUserData(user, userRoles: new List<string> { SD.Role_User }, jwtSecurityToken, refreshToken);
    }

    public async Task<AuthDto> GetTokenAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        
        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return new AuthDto { Message = "Email or Password is incorrect!" };

        var jwtSecurityToken = await _tokenHandler.CreateJwtToken(user);
        
        var refreshToken = await _tokenHandler.CreateRefreshToken(user);

        return _mapAuthUserData(user, userRoles: (await _userManager.GetRolesAsync(user)).ToList(),
            jwtSecurityToken, refreshToken);
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
            authDto.Message = "Invalid Token!";
            return authDto;
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
       
        if (!refreshToken.IsActive)
        {
            authDto.Message = "Inactive Token!";
            return authDto;
        }

        //step 1 : revoke refreshToken
        refreshToken.RevokedOn = DateTime.UtcNow;

        //step 2 : create new refreshToken
        var newRefreshToken = await _tokenHandler.CreateRefreshToken(user);

        //step 3 : Create JWT Token 
        var jwtToken = await _tokenHandler.CreateJwtToken(user);

        return _mapAuthUserData( user, userRoles: (await _userManager.GetRolesAsync(user)).ToList(),
            jwtToken, refreshToken);
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
    #endregion

    #region Helper Method
    private AuthDto _mapAuthUserData(ApplicationUser user, List<string> userRoles,
        JwtSecurityToken JwtToken, RefreshToken refreshToken)
    {
        return new AuthDto
        {
            IsAuthenticated = true,
            Email = user.Email,
            UserName = user.UserName,
            Roles = userRoles,
            Token = new JwtSecurityTokenHandler().WriteToken(JwtToken),
            ExpiresOn = JwtToken.ValidTo,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresOn,
        };
    }
    #endregion
}
