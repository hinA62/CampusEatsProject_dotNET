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

        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? config["Jwt:Key"];
        
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException(
                "JWT Key is not configured! " +
                "Set JWT_KEY environment variable or Jwt:Key in appsettings.");
        }

        // Validare lungime minimă pentru securitate
        if (jwtKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT Key must be at least 32 characters long for security.");
        }

        // Log pentru debugging (fără a expune cheia)
        var keySource = Environment.GetEnvironmentVariable("JWT_KEY") != null 
            ? "Environment Variable (JWT_KEY) ✓ Production-ready" 
            : jwtKey.Contains("DEV_ONLY") 
                ? "appsettings.Development.json ⚠ Development only" 
                : "Configuration file";
        logger.LogInformation("JWT Key loaded from: {KeySource}", keySource);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        // SonarQube suppression: jwtKey provine din Environment Variable (producție)
        // sau appsettings.Development.json (development only, exclus din Git prin .gitignore)
#pragma warning disable S6418 // Hard-coded secrets
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
#pragma warning restore S6418
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