using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CampusEats.Features.User;
using Microsoft.Extensions.Logging;

namespace CampusEats.Features.Auth;

public class JwtService(IConfiguration config, ILogger<JwtService> logger)
{
    public string GenerateToken(User.User user)
    {
        // Validare: verifică că cheia JWT este configurată
        var jwtKey = config["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key is not configured! Please set Jwt:Key in appsettings or as environment variable.");
        }

        // Log pentru debugging (fără a expune cheia)
        var keySource = jwtKey.Contains("DEV_ONLY") ? "appsettings.Development.json" : "Environment Variable or Production Config";
        logger.LogInformation("JWT Key loaded from: {KeySource}", keySource);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(int.Parse(config["Jwt:ExpiryMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}