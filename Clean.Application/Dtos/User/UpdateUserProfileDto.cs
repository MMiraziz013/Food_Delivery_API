namespace Clean.Application.Dtos.User;

public class UpdateUserProfileDto
{
    public string Username { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
}