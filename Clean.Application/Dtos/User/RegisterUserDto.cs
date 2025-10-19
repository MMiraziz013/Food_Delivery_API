using System.ComponentModel.DataAnnotations;

namespace Clean.Application.Dtos.User;

public class RegisterUserDto
{
    public string Username { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }
    
    public string Address { get; set; }
    
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    public string Email { get; set; }
}