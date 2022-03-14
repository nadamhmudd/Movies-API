using Microsoft.EntityFrameworkCore;

namespace Movies.Core.Entities.Models;

[Owned]
public class RefreshToken
{
    #region Props
    public string Token { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresOn { get; set; } = DateTime.UtcNow.AddDays(10);
    public DateTime? RevokedOn { get; set; }
    #endregion

    #region Methods
    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public bool IsActive => RevokedOn == null && !IsExpired;
    #endregion
}
