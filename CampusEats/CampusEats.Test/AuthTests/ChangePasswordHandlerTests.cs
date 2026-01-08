using CampusEats.Features.Auth.Handlers;
using CampusEats.Features.Auth.Requests;
using CampusEats.Features.User;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Test.AuthTests;

public class ChangePasswordHandlerTests
{
    private readonly CampusEatsContext _context;
    private readonly ChangePasswordHandler _handler;

    public ChangePasswordHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new CampusEatsContext(options);
        _handler = new ChangePasswordHandler(_context);
    }

    [Fact]
    public async Task Handle_ValidPasswordChange_Success()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "test",
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("OldPass123"),
            Role = UserRole.Client,
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new ChangePasswordRequest(userId, "OldPass123", "NewPass456");

        var result = await _handler.Handle(request);

        result.Should().NotBeNull();
        var updatedUser = await _context.Users.FindAsync(userId);
        BCrypt.Net.BCrypt.Verify("NewPass456", updatedUser!.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_IncorrectCurrentPassword_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "test",
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("RealPassword"),
            Role = UserRole.Client,
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new ChangePasswordRequest(userId, "WrongPassword", "NewPass456");

        var result = await _handler.Handle(request);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        var request = new ChangePasswordRequest(Guid.NewGuid(), "OldPass", "NewPass");

        var result = await _handler.Handle(request);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_PasswordNotChanged_WhenWrongCurrent()
    {
        var userId = Guid.NewGuid();
        var originalHash = BCrypt.Net.BCrypt.HashPassword("Original123");
        var user = new User
        {
            Id = userId,
            Username = "test",
            Email = "test@test.com",
            PasswordHash = originalHash,
            Role = UserRole.Client,
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new ChangePasswordRequest(userId, "WrongPass", "NewPass456");
        await _handler.Handle(request);

        var unchanged = await _context.Users.FindAsync(userId);
        unchanged!.PasswordHash.Should().Be(originalHash);
    }
}
