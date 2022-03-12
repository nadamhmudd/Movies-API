namespace Movies.Core.DTOs;
public class RegisterDto
{
    [StringLength(50)]
    public string UserName { get; set; }

    [StringLength(128)]
    public string Email { get; set; }

    [MaxLength(50)]
    public string FirstName { get; set; }

    [MaxLength(50)]
    public string LastName { get; set; }

    [StringLength(256)]
    public string Password { get; set; }
}
