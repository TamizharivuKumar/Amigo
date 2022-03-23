using Amigo.Entities;

namespace Amigo.Models;

public class AuthResponse
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public Role Role { get; set; }
    public string? Token { get; set; }

    public AuthResponse(Admin admin, string token)
    {
        Id = admin.Adminid;
        Username = admin.Adminname;
        Role = admin.Role;
        Token = token;
    }
};