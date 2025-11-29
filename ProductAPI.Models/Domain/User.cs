using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Models.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    public string PasswordHash { get; set; }

    public string? RefreshToken { get; set; } = string.Empty;

    public DateTime TokenCreated { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }

}

