using Clean.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Clean.Domain.Entities;

public class User : IdentityUser<int>
{
    public string Name { get; set; }
    public string Address { get; set; }
    public DateTime RegistrationDate { get; set; }
    public UserRole Role { get; set; }

    public List<Order> Orders { get; set; } = new List<Order>();
}