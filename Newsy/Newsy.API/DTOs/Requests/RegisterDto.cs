using Newsy.API.Attributes;

namespace Newsy.API.DTOs.Requests;

public class RegisterDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public bool IsAuthor { get; set; }

    [RequiredIf(nameof(IsAuthor))]
    public string? Bio { get; set; }
}
