namespace Movies.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    #region Props
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnviroment;
    private readonly IFileHandler _fileHandler;
    private readonly IMapper _mapper;
    #endregion

    #region Constructor(s)
    public MoviesController(IUnitOfWork unitOfWork,
        IWebHostEnvironment hostEnviroment,
        IFileHandler fileHandler,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _hostEnviroment = hostEnviroment;
        _fileHandler = fileHandler;
        _mapper = mapper;
    }
    #endregion

    #region Actions
    [HttpGet]
    public async Task<IActionResult> GetAllAysnc()
    {
        var movies = await _unitOfWork.Movie.GetAllAsync(
            includeProperties: "Genre", 
            orderBy: m => m.Rate,
            orderByDirection: SD.Descending);

        return Ok(_mapper.Map<IEnumerable<MovieDetailsDto>>(movies));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var movie = await _unitOfWork.Movie.GetFirstOrDefaultAsync(
            criteria: m => m.Id == id,
            includeProperties: "Genre");

        if (movie is null)
            return NotFound($"No movie was found with ID: {id}");

        return Ok(_mapper.Map<MovieDetailsDto>(movie));
    }

    [HttpGet("GetByGenreId")]
    public async Task<IActionResult> GetByGenreIdAsync(int genreId)
    {
        if (!await _unitOfWork.Genre.IsValidAsync(g => g.Id == genreId))
            return BadRequest("Invaild Genre ID!");

        var movies = await _unitOfWork.Movie.GetAllAsync(
            criteria: m => m.GenreId == genreId,
            includeProperties: "Genre",
            orderBy: m => m.Rate, orderByDirection: SD.Descending);

        if (movies.Count() == 0)
            return NotFound($"No movie was found under genre: {(await _unitOfWork.Genre.GetFirstOrDefaultAsync(g => g.Id == genreId)).Name}");

        return Ok(_mapper.Map<IEnumerable<MovieDetailsDto>>(movies));
    }

    [HttpPost, Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> CreateAsync([FromForm] MovieCreateionDto dto)
    {
        if (!await _unitOfWork.Genre.IsValidAsync(g => g.Id == dto.GenreId))
            return BadRequest("Invaild Genre ID!");

        //var x = 5;
        string posterUrl = await _fileHandler.Image.Upload(dto.Poster, Path.Combine(_hostEnviroment.WebRootPath, SD.MoviesPosterpath));
        if (!posterUrl.Contains('\\')) //not path
            return BadRequest(posterUrl); //return error message

        //mapping 
        var movie = _mapper.Map<Movie>(dto);
        movie.PosterUrl = posterUrl;

        await _unitOfWork.Movie.AddAsync(movie);
        _unitOfWork.Save();

        return Ok(movie);
    }

    [HttpPut("{id}"), Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieUpdateDto dto)
    {
        var movie = await _unitOfWork.Movie.GetByIdAsync(id);

        if (movie is null)
            return NotFound($"No movie was found with ID: {id}");

        if (!await _unitOfWork.Genre.IsValidAsync(g => g.Id == dto.GenreId))
            return BadRequest("Invaild Genre ID!");

        if (dto.Poster is not null)
        {
            //delete old poster
            _fileHandler.Image.Delete(movie.PosterUrl);

            //upload new poster
            var newPosterUrl = await _fileHandler.Image.Upload(dto.Poster, Path.Combine(_hostEnviroment.WebRootPath, SD.MoviesPosterpath));

            if (!newPosterUrl.Contains('\\')) //not path
                return BadRequest(newPosterUrl); //return error message

            movie.PosterUrl = newPosterUrl;
        }
        movie.GenreId = dto.GenreId;
        movie.Title = dto.Title.Trim().CapitalizeFistLitter();
        movie.Rate = dto.Rate;
        movie.StoryLine = dto.StoryLine;
        movie.Year = dto.Year;

        _unitOfWork.Movie.Update(movie);
        _unitOfWork.Save();

        return Ok(movie);
    }

    [HttpDelete("{id}"), Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var movie = await _unitOfWork.Movie.GetByIdAsync(id);

        if (movie is null)
            return NotFound($"No movie was found with ID: {id}");

        //Delete it
        //first delete poster image
        _fileHandler.Image.Delete(movie.PosterUrl);
        _unitOfWork.Movie.Delete(movie);
        _unitOfWork.Save();

        return Ok(movie);
    }
    #endregion
}