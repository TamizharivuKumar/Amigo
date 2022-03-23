using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Amigo.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace Amigo.Helpers;

public class JwtHelper
{
    private readonly RequestDelegate? _next;
    private readonly AppSettings? _appSettings;

    public JwtHelper(RequestDelegate next, IOptions<AppSettings> appsettings)
    {
        _next = next;
        _appSettings = appsettings.Value;
    }

    public async Task Invoke(HttpContext context, IUserServices userServices)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            attachUserToContext(userServices, context, token);
        await _next!(context);
    }

    private void attachUserToContext(IUserServices userServices, HttpContext context, string token)
    {
        try
        {
            var key = Encoding.ASCII.GetBytes(_appSettings!.Secret!);
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                },
                out SecurityToken validateToken
            );

            var jwtToken = (JwtSecurityToken)validateToken;
            var userID = int.Parse(jwtToken.Claims.FirstOrDefault(x => x.Type == "Id")!.Value);

            context.Items["User"] = userServices.GetById(userID);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}