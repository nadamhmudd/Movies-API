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

    [HttpGet] 
    public async Task<IActionResult> GetAllAysnc()
    {
        var movies = await _unitOfWork.Movie.GetAllAsync(includeProperties: "Genre", orderBy: m => m.Rate, orderByDirection: SD.Descending);
        
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var movie = await _unitOfWork.Movie.GetFirstOrDefaultAsync(
            criteria: m => m.Id == id,
            includeProperties: "Genre");

        if (movie is null)
            return NotFound($"No movie was found with ID: {id}");

        return Ok(movie);
    }

    [HttpGet("GetByGenreId")]
    public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
    {
        if(! await _unitOfWork.Genre.IsValidAsync(g => g.Id == genreId))
            return BadRequest("Invaild Genre ID!");

        var movie = await _unitOfWork.Movie.GetAllAsync(
            criteria: m => m.GenreId == genreId,
            includeProperties: "Genre",
            orderBy: m => m.Rate, orderByDirection: SD.Descending);

        if (movie.Count() == 0)
            return NotFound($"No movie was found under genre: {(await _unitOfWork.Genre.GetFirstOrDefaultAsync(g => g.Id==genreId)).Name}");

        return Ok(movie);
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var movie = await _unitOfWork.Movie.GetByIdAsync(id);

        if (movie is null)
            return NotFound($"No movie was found with ID: {id}");

        //Delete it
        //first delete poster image
        ImagesHelper.DeleteImage(movie.PosterUrl);
        _unitOfWork.Movie.Delete(movie);
        _unitOfWork.Save();

        return Ok(movie);
    }
}
