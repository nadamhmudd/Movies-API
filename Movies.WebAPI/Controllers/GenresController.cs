namespace Movies.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GenresController : ControllerBase
{
    #region Props
    private readonly IUnitOfWork _unitOfWork;
    #endregion

    #region Constructor(s)
    public GenresController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    #endregion

    #region Actions
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var genres = await _unitOfWork.Genre.GetAllAsync(orderBy: g => g.Name);
        
        return Ok(genres);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(byte id)
    {
        var genre = await _unitOfWork.Genre.GetFirstOrDefaultAsync(g => g.Id == id);

        if (genre is null)
            return NotFound($"No genre was found with ID: {id}");

        return Ok(genre);
    }

    [HttpPost, Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> CreateAsync(GenreDto dto)
    {
        var genre = new Genre
        {
            Name = dto.Name.Trim().CapitalizeFistLitter() //Extension Method for String
        };
        await _unitOfWork.Genre.AddAsync(genre);
        _unitOfWork.Save();

        return Ok(genre);
    }

    [HttpPut("{id}"), Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> UpdateAsync(byte id, [FromBody] GenreDto dto)
    {
        var genre = await _unitOfWork.Genre.GetFirstOrDefaultAsync(g => g.Id == id);

        if (genre is null)
            return NotFound($"No genre was found with ID: {id}");

        //update genre , tracked = false
        genre.Name = dto.Name.Trim().CapitalizeFistLitter();
        _unitOfWork.Genre.Update(genre);
        _unitOfWork.Save();

        return Ok(genre);
    }

    [HttpDelete("{id}"), Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> DeleteAsync(byte id)
    {
        var genre = await _unitOfWork.Genre.GetFirstOrDefaultAsync(g => g.Id == id);

        if (genre is null)
            return NotFound($"No genre was found with ID: {id}");

        //Delete
        _unitOfWork.Genre.Delete(genre);
        _unitOfWork.Save();

        return Ok(genre);
    }
    #endregion
}
