using CampusEats.Features.Auth;
using CampusEats.Features.User;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CampusEats.Test.AuthTests;

public class JwtServiceTests
{
    private readonly JwtService _service;

    public JwtServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"Jwt:Key", "TestSecretKey1234567890123456789012345678901234567890"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"},
                {"Jwt:ExpiryMinutes", "60"}
            }!)
            .Build();

        _service = new JwtService(config);
    }

    [Fact]
    public void GenerateToken_ValidUser_ReturnsToken()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@test.com",
            Role = UserRole.Client,
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };

        var token = _service.GenerateToken(user);

        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(token).Should().BeTrue();
    }

    [Fact]
    public void GenerateToken_ContainsCorrectClaims()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "admin",
            Email = "admin@test.com",
            Role = UserRole.Admin,
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };

        var token = _service.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "admin");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == "admin@test.com");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    [Fact]
    public void GenerateToken_HasCorrectExpiry()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "test",
            Email = "test@test.com",
            Role = UserRole.Client,
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };

        var token = _service.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var expectedExpiry = DateTime.UtcNow.AddMinutes(60);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiry, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void GenerateToken_ForDifferentRoles_GeneratesUniqueTokens()
    {
        var client = new User { Id = Guid.NewGuid(), Username = "client", Email = "c@test.com", Role = UserRole.Client, PasswordHash = "h", CreatedAt = DateTime.UtcNow };
        var admin = new User { Id = Guid.NewGuid(), Username = "admin", Email = "a@test.com", Role = UserRole.Admin, PasswordHash = "h", CreatedAt = DateTime.UtcNow };

        var tokenClient = _service.GenerateToken(client);
        var tokenAdmin = _service.GenerateToken(admin);

        tokenClient.Should().NotBe(tokenAdmin);
    }
}
