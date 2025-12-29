using CampusEats.Features.Menu;
using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.MenuTests.IntegrationTests;

public class CreateMenuHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly CreateMenuHandler _handler;

    public CreateMenuHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"MenuCoverageDb_{Guid.NewGuid()}")
            .Options;
        _context = new CampusEatsContext(options);
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CreateMenuHandler>();
        _handler = new CreateMenuHandler(_context, logger);
    }
    
    [Fact]
    public async Task Given_ValidCreateMenuRequest_When_Handle_Then_ShouldCreateMenu()
    {
        // Arrange
        var request = new CreateMenuRequest(
            Guid.NewGuid(),
            Name: "Vegan Delight",
            Price: 12.99m,
            ItemIds: [Guid.NewGuid(), Guid.NewGuid()],
            Category: MenuCategory.Vegan,
            ImageUrl: null
        );
        
        // Act
        var result = await _handler.Handle(request);
        var createdMenu = await _context.Menu.FirstOrDefaultAsync(m => m.Name == "Vegan Delight");
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(createdMenu);
        Assert.Equal("Vegan Delight", createdMenu.Name);
        Assert.Equal(12.99m, createdMenu.Price);
        Assert.Equal(MenuCategory.Vegan, createdMenu.Category);
        Assert.Null(createdMenu.ImageUrl);
    }

    [Fact]
    public async Task Given_ValidationFailure_When_Handle_Then_ShouldReturnBadRequest()
    {
        //Arrange
        var request = new CreateMenuRequest(Guid.Empty, "Vegan Delight", 
            12.99m, [Guid.NewGuid()], MenuCategory.Vegan, null);
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("lapte", DietaryRestrictions.GlutenFree |
                         DietaryRestrictions.NutFree |
                         DietaryRestrictions.DairyFree |
                         DietaryRestrictions.NoSeafood)]
    [InlineData("gluten", DietaryRestrictions.LactoseFree |
                          DietaryRestrictions.NutFree |
                          DietaryRestrictions.DairyFree |
                          DietaryRestrictions.NoSeafood)]
    [InlineData("alune", DietaryRestrictions.LactoseFree |
                         DietaryRestrictions.GlutenFree |
                         DietaryRestrictions.DairyFree |
                         DietaryRestrictions.NoSeafood)]
    [InlineData("brânză", DietaryRestrictions.LactoseFree |
                          DietaryRestrictions.GlutenFree |
                          DietaryRestrictions.NutFree |
                          DietaryRestrictions.NoSeafood)]
    [InlineData("pește", DietaryRestrictions.LactoseFree |
                         DietaryRestrictions.GlutenFree |
                         DietaryRestrictions.NutFree |
                         DietaryRestrictions.DairyFree)]
    [InlineData("nuci,lactoză,fructe de mare",
        DietaryRestrictions.GlutenFree | DietaryRestrictions.DairyFree)]
    public async Task Handle_WhenValid_CalculatesRestrictionsCorrectly(string allergensCsv,
        DietaryRestrictions expected)
    {
        // Arrange
        var allergens = allergensCsv.Split(',').ToList();
        var itemId = Guid.NewGuid();
        var menuItem = new MenuItem(itemId, "Ingredient", 0, null, allergens);

        await _context.MenuItem.AddAsync(menuItem);
        await _context.SaveChangesAsync();

        var request = new CreateMenuRequest(Guid.NewGuid(), "Menu Test", 15m,
            [itemId], MenuCategory.Meat, null);

        // Act
        var result = await _handler.Handle(request);

        // Assert
        var createdResult = result.Should().BeOfType<Created<Menu>>().Subject;
        createdResult.Value?.Restrictions.Should().Be(expected);

        var dbMenu = await _context.Menu.FirstOrDefaultAsync(m => m.Name == "Menu Test");
        dbMenu.Should().NotBeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}