namespace Movies.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto dto)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _unitOfWork.Auth.RegisterAsync(dto);

        if (!user.IsAuthenticated)
            return BadRequest(user.Message);

        return Ok(user);
    }

}
