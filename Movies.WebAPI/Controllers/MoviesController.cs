﻿namespace Movies.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnviroment;
    //private readonly IBaseFileHandler _fileHandler;
    private readonly IMapper _mapper;

    public MoviesController(IUnitOfWork unitOfWork,
        IWebHostEnvironment hostEnviroment,
        //IBaseFileHandler fileHandler
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _hostEnviroment = hostEnviroment;
        //_fileHandler = fileHandler;
        _mapper = mapper;
    }

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

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] MovieCreateionDto dto)
    {
        if (!await _unitOfWork.Genre.IsValidAsync(g => g.Id == dto.GenreId))
            return BadRequest("Invaild Genre ID!");

        //var x = 5;
        string posterUrl = await BaseFileHandler.Upload(dto.Poster, Path.Combine(_hostEnviroment.WebRootPath, SD.MoviesPosterpath));
        if (!posterUrl.Contains('\\')) //not path
            return BadRequest(posterUrl); //return error message

        //mapping 
        var movie = _mapper.Map<Movie>(dto);
        movie.PosterUrl = posterUrl;

        await _unitOfWork.Movie.AddAsync(movie);
        _unitOfWork.Save();

        return Ok(movie);
    }

    [HttpPut("{id}")]
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
            BaseFileHandler.Delete(movie.PosterUrl);

            //upload new poster
            var newPosterUrl = await BaseFileHandler.Upload(dto.Poster, Path.Combine(_hostEnviroment.WebRootPath, SD.MoviesPosterpath));

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var movie = await _unitOfWork.Movie.GetByIdAsync(id);

        if (movie is null)
            return NotFound($"No movie was found with ID: {id}");

        //Delete it
        //first delete poster image
        BaseFileHandler.Delete(movie.PosterUrl);
        _unitOfWork.Movie.Delete(movie);
        _unitOfWork.Save();

        return Ok(movie);
    }
}