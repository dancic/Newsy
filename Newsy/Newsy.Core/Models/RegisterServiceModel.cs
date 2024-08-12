namespace Newsy.Core.Models;
public class RegisterServiceModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public bool IsAuthor { get; set; }
    public string? Bio { get; set; }
}