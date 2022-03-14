using System.Text.Json.Serialization;

namespace Movies.Core.DTOs;
public class AuthDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresOn { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }

    #region  Ignored props
    [JsonIgnore] public string Message { get; set; }
    [JsonIgnore] public bool IsAuthenticated { get; set; } //by default = false
    [JsonIgnore] public string? RefreshToken { get; set; } //send it in cookie
    #endregion
}
