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
    private readonly IJWT _jwt;

    public AuthRepository(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IJWT jwt, IMapper mapper)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._jwt = jwt;
        _mapper = mapper;
    }

    public async Task<AuthDto> RegisterAsync(RegisterDto dto)
    {
        if(await _userManager.FindByEmailAsync(dto.Email) is not null)
            return new AuthDto { Message = "Email is already registered!" };

        //if(await _userManager.FindByNameAsync(dto.UserName) is not null)
        //    return new AuthDto { Message = "Username is already registered!" };

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
        var jwtSecurityToken = await _jwt.CreateJwtToken(user);

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

    public async Task<AuthDto> GetTokenAsync(TokenRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        
        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return new AuthDto { Message = "Email or Password is incorrect!" };


        var jwtSecurityToken = await _jwt.CreateJwtToken(user);
        
        var rolesList = await _userManager.GetRolesAsync(user);

        return  new AuthDto
        {
            Email = user.Email,
            UserName = user.UserName,
            Roles = rolesList.ToList(),
            IsAuthenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            ExpiresOn = jwtSecurityToken.ValidTo
        };
    }
}
