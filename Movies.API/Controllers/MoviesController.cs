namespace Movies.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnviroment;
    public MoviesController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnviroment = null)
    {
        _unitOfWork = unitOfWork;
        _hostEnviroment = hostEnviroment;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
    {
        string posterUrl = ImagesHelper.UploadImage(dto.Poster, Path.Combine(_hostEnviroment.WebRootPath, SD.MoviesPosterpath));

        if (!posterUrl.Contains('\\')) //not path
            return BadRequest(posterUrl); //return error message

        if (! await _unitOfWork.Genre.IsValidAsync(g => g.Id == dto.GenreId))
            return BadRequest("Invaild Genre ID!");

        var movie = new Movie
        {
            GenreId = dto.GenreId,
            Title = dto.Title.Trim().CapitalizeFistLitter(),
            Rate = dto.Rate,
            StoryLine = dto.StoryLine,
            Year = dto.Year,
            PosterUrl = posterUrl
        };
        await _unitOfWork.Movie.AddAsync(movie);
        _unitOfWork.Save();

       return Ok(movie);
    }
}
