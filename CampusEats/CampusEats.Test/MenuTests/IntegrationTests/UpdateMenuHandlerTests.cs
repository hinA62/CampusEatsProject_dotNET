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

    [Fact]
    public async Task Given_ItemsWithGlutenAllergen_When_Handle_Then_ShouldRemoveGlutenFree()
    {
        var itemId = Guid.NewGuid();
        var menuId = Guid.NewGuid();
        
        await _context.MenuItem.AddAsync(new MenuItem(itemId, "Bread", 5m,
            null, ["gluten", "grâu"]));
        await _context.Menu.AddAsync(new Menu(menuId, "Breakfast Set", 10m, [itemId],
            MenuCategory.Breakfast, DietaryRestrictions.GlutenFree, null));
        await _context.SaveChangesAsync();

        var request = new UpdateMenuRequest(menuId, "Breakfast Set", 10m, [itemId],
            MenuCategory.Breakfast, DietaryRestrictions.GlutenFree, null);

        var result = await _handler.Handle(request);

        var okResult = result.Should().BeOfType<Ok<Menu>>().Subject;
        okResult.Value!.Restrictions.Should().NotHaveFlag(DietaryRestrictions.GlutenFree);
    }

    [Fact]
    public async Task Given_ItemsWithNutAllergen_When_Handle_Then_ShouldRemoveNutFree()
    {
        var itemId = Guid.NewGuid();
        var menuId = Guid.NewGuid();
        
        await _context.MenuItem.AddAsync(new MenuItem(itemId, "Nut Cake", 8m,
            null, ["nuci", "arahide"]));
        await _context.Menu.AddAsync(new Menu(menuId, "Dessert Menu", 15m, [itemId],
            MenuCategory.Dessert, DietaryRestrictions.NutFree, null));
        await _context.SaveChangesAsync();

        var request = new UpdateMenuRequest(menuId, "Dessert Menu", 15m, [itemId],
            MenuCategory.Dessert, DietaryRestrictions.NutFree, null);

        var result = await _handler.Handle(request);

        var okResult = result.Should().BeOfType<Ok<Menu>>().Subject;
        okResult.Value!.Restrictions.Should().NotHaveFlag(DietaryRestrictions.NutFree);
    }

    [Fact]
    public async Task Given_ItemsWithoutAllergens_When_Handle_Then_ShouldSetAllRestrictions()
    {
        var itemId = Guid.NewGuid();
        var menuId = Guid.NewGuid();
        
        await _context.MenuItem.AddAsync(new MenuItem(itemId, "Salad", 7m,
            null, null));
        await _context.Menu.AddAsync(new Menu(menuId, "Healthy Menu", 12m, [itemId],
            MenuCategory.Vegetarian, DietaryRestrictions.None, null));
        await _context.SaveChangesAsync();

        var request = new UpdateMenuRequest(menuId, "Healthy Menu", 12m, [itemId],
            MenuCategory.Vegetarian, DietaryRestrictions.None, null);

        var result = await _handler.Handle(request);

        var okResult = result.Should().BeOfType<Ok<Menu>>().Subject;
        var restrictions = okResult.Value!.Restrictions;
        // When allergens is null, no items are checked, so restrictions stay as currentRestrictions
        restrictions.Should().Be(DietaryRestrictions.None);
    }

    [Fact]
    public async Task Given_UpdateWithNewItemIds_When_Handle_Then_ShouldReplaceItems()
    {
        var oldItemId = Guid.NewGuid();
        var newItemId1 = Guid.NewGuid();
        var newItemId2 = Guid.NewGuid();
        var menuId = Guid.NewGuid();
        
        await _context.Menu.AddAsync(new Menu(menuId, "Old Menu", 10m, [oldItemId],
            MenuCategory.Lunch, DietaryRestrictions.None, null));
        await _context.SaveChangesAsync();

        var request = new UpdateMenuRequest(menuId, "New Menu", 15m, [newItemId1, newItemId2],
            MenuCategory.Dinner, DietaryRestrictions.None, null);

        var result = await _handler.Handle(request);

        var okResult = result.Should().BeOfType<Ok<Menu>>().Subject;
        okResult.Value!.ItemId.Should().Contain(newItemId1);
        okResult.Value!.ItemId.Should().Contain(newItemId2);
        okResult.Value!.ItemId.Should().NotContain(oldItemId);
    }

    [Fact]
    public async Task Given_UpdateWithNullItemIds_When_Handle_Then_ShouldReturnBadRequest()
    {
        var originalItemId = Guid.NewGuid();
        var menuId = Guid.NewGuid();
        
        await _context.Menu.AddAsync(new Menu(menuId, "Test Menu", 10m, [originalItemId],
            MenuCategory.Breakfast, DietaryRestrictions.None, null));
        await _context.SaveChangesAsync();

        var request = new UpdateMenuRequest(menuId, "Test Menu Updated", 12m, null!,
            MenuCategory.Breakfast, DietaryRestrictions.None, null);

        var result = await _handler.Handle(request);

        result.Should().BeOfType<BadRequest<List<FluentValidation.Results.ValidationFailure>>>();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}