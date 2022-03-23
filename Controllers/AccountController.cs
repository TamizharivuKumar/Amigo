
using Amigo.Helpers;
using Amigo.Models;
using Amigo.Services;
using Amigo.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Amigo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly AmigoContext _context;
    private readonly IUserServices _userServices;

    public AccountController(AmigoContext context, IUserServices userServices)
    {
        _context = context;
        _userServices = userServices;
    }
    
    [HttpPost]
    public IActionResult Login(AuthRequest model)
    {
        var admin = _context.Admin.FirstOrDefault(x => x.Adminemail == model.Email);
        if (admin == null)
            return BadRequest(new { message = "Email not found!" });
        var verify = BCrypt.Net.BCrypt.Verify(model.Password, admin!.Adminpassword);
        if (!verify)
            return BadRequest(new { message = "Incorrect password!" });
        var response = _userServices.Authenticate(admin);
        return Ok(response);
    }

    [Authorize(Role.Product)]
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _userServices.GetAll();
        return Ok(users);
    }
}