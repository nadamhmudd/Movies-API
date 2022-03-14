namespace Movies.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    #region Props
    private readonly IUnitOfWork _unitOfWork;
    #endregion

    #region Constructor(s)
    public AuthController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    #endregion

    #region Actions

    [HttpGet("refreshToken")] //refresh token for loggeduser
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies[SD.Cookies_RefreshToken];

        var result = await _unitOfWork.Auth.RefreshTokenAsync(refreshToken);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto dto)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _unitOfWork.Auth.RegisterAsync(dto);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

        return Ok(result);
    }

    [HttpPost("login")] //request to get token
    public async Task<IActionResult> GetTokenAsync([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _unitOfWork.Auth.GetTokenAsync(dto);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        if (!string.IsNullOrEmpty(result.RefreshToken))
            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

        return Ok(result);
    }

    [HttpPost("revokeToken")]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto dto)
    {
        var token = dto.Token ?? Request.Cookies[SD.Cookies_RefreshToken];

        if (string.IsNullOrEmpty(token))
            return BadRequest("Token is required");

        var result = await _unitOfWork.Auth.RevokeTokenAsync(token);

        if (!result)
            return BadRequest("Token is Invalid");

        return Ok();
    }

    [HttpPost("addrole"), Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _unitOfWork.Auth.AddRoleAsync(dto);

        if (!string.IsNullOrEmpty(result))
            return BadRequest(result);

        return Ok(dto);
    }

    #endregion

    #region Helper Mehods
    private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expires.ToLocalTime()
        };
        Response.Cookies.Append(SD.Cookies_RefreshToken, refreshToken, cookieOptions);
    }
    #endregion
}

