using CampusEats.Features.Menu;
using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using CampusEats.Validators.Menu;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.MenuTests.IntegrationTests;

public class UpdateMenuHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly UpdateMenuHandler _handler;
    private readonly ILogger<UpdateMenuHandler> _logger = new LoggerFactory().CreateLogger<UpdateMenuHandler>();

    public UpdateMenuHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new CampusEatsContext(options);
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<UpdateMenuHandler>();
        var validator = new UpdateMenuValidator();
        _handler = new UpdateMenuHandler(_context, logger, validator);
    }

    [Fact]
    public async Task Given_ValidUpdateMenuRequest_When_Handle_Then_ShouldUpdateMenu()
    {
        //Arrange
        var menuId = Guid.NewGuid();
        var existingMenu = new Menu(menuId, "Old Menu", 10.00m, [Guid.NewGuid()],
            MenuCategory.Breakfast, DietaryRestrictions.None, null);
        await _context.Menu.AddAsync(existingMenu);
        await _context.SaveChangesAsync();
        
        var request = new UpdateMenuRequest(menuId, "New Menu", 12.50m,
            [Guid.NewGuid(), Guid.NewGuid()], MenuCategory.Lunch, DietaryRestrictions.GlutenFree, null);
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        var okResult = result.Should().BeOfType<Ok<Menu>>().Subject;
        okResult.Value!.Name.Should().Be("New Menu");
        okResult.Value.Price.Should().Be(12.50m);
    }

    [Fact]
    public async Task Given_ValidationFailure_When_Handle_Then_ShouldReturnBadRequest()
    {
        //Assert
        var request = new UpdateMenuRequest(Guid.Empty, "a", -5.0m,
            [], MenuCategory.Dessert, DietaryRestrictions.None, null);
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        result.Should().BeOfType<BadRequest<List<FluentValidation.Results.ValidationFailure>>>();
    }
    
    [Fact]
    public async Task Given_NonExistentMenuId_When_Handle_Then_ShouldReturnNotFound()
    {
        //Arrange
        var request = new UpdateMenuRequest(Guid.NewGuid(), "Updated Menu", 15.00m,
            [Guid.NewGuid()], MenuCategory.Dinner, DietaryRestrictions.NutFree, null);
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        result.Should().BeOfType<NotFound<string>>();
    }

    [Fact]
    public async Task Given_EmptyAllergensList_When_Handle_Then_ShouldCalculateNoRestrictions()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var initialRestrictions = DietaryRestrictions.GlutenFree;
        var itemId = Guid.NewGuid();
        await _context.Menu.AddAsync(new Menu(menuId, "Test", 10m, [itemId],
            MenuCategory.Meat, initialRestrictions, null));
        await _context.SaveChangesAsync();
        
        var request = new UpdateMenuRequest(menuId, "Test", 10m, [itemId],
            MenuCategory.Meat, DietaryRestrictions.None, "");

        // Act
        var result = await _handler.Handle(request);

        // Assert
        var okResult = result.Should().BeOfType<Ok<Menu>>().Subject;
        okResult.Value!.Restrictions.Should().Be(initialRestrictions);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}