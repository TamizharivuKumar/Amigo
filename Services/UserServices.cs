using System.IdentityModel.Tokens.Jwt;
using Amigo.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Amigo.Helpers;

namespace Amigo.Services;

public interface IUserServices
{
    AuthResponse Authenticate(Admin model);
    IEnumerable<Admin> GetAll();
    Admin GetById(int Id);
}

public class UserServices : IUserServices
{
    private readonly AmigoContext _context;

    private readonly AppSettings _appSettings;

    public UserServices(AmigoContext context, IOptions<AppSettings> appSettings)
    {
        _context = context;
        _appSettings = appSettings.Value;
    }

    public AuthResponse Authenticate(Admin admin)
    {       
        var token = GenerateJwtToken(admin);
        return new AuthResponse(admin, token);
    }

    public IEnumerable<Admin> GetAll()
    {
        return _context.Admin.ToList();
    }

    public Admin GetById(int Id)
    {
        return _context.Admin.FirstOrDefault(x => x.Adminid == Id)!;
    }

    private string GenerateJwtToken(Admin user)
    {
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim("Id", user.Adminid.ToString())
                }
            ),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}