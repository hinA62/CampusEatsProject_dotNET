using CampusEats.Features.Menu;
using CampusEats.Features.Menu.Requests;
using CampusEats.Validators.Menu;

namespace CampusEats.Test.MenuTests;

public class CreateMenuValidatorTests
{
    [Fact]
    public void Given_ValidInput_When_Validate_Then_ShouldPass()
    {
        // Arrange
        var model = new CreateMenuRequest
            (Guid.NewGuid(),"Valid Menu Name", 15.99m, [Guid.NewGuid(), Guid.NewGuid()],
                MenuCategory.Vegan, "http://example.com/image.jpg");
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    public void Given_NullOrEmptyName_When_Validate_Then_ShouldFail(string name)
    {
        // Arrange
        var model = new CreateMenuRequest
            (Guid.NewGuid(), name, 15.99m, [Guid.NewGuid()], 
                MenuCategory.Vegetarian, null);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Menu name is required.");
    }

    [Fact]
    public void Given_NameExceeds50Characters_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var longName = new string('a', 51);
        var model = new CreateMenuRequest
            (Guid.NewGuid(), longName, 15.99m, [Guid.NewGuid()],
                MenuCategory.Traditional, null);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Menu name cannot exceed 50 characters.");
    }

    [Fact]
    public void Given_NullPrice_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new CreateMenuRequest
            (Guid.NewGuid(), "Valid Menu", null, [Guid.NewGuid()],
                MenuCategory.Asian, null);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Price is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.99)]
    public void Given_ZeroOrNegativePrice_When_Validate_Then_ShouldFail(decimal price)
    {
        // Arrange
        var model = new CreateMenuRequest
            (Guid.NewGuid(), "Valid Menu", price, [Guid.NewGuid()], 
                MenuCategory.Vegan, null);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Price must be greater than zero.");
    }

    [Theory]
    [InlineData(null!)]
    public void Given_NullOrEmptyItemIds_When_Validate_Then_ShouldFail(List<Guid>? itemIds)
    {
        // Arrange
        var model = new CreateMenuRequest
        ( Guid.NewGuid(), "Valid Menu", 15.99m, itemIds!,
            MenuCategory.Dessert, null);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == "At least one menu item must be selected.");
    }

    [Fact]
    public void Given_MultipleInvalidFields_When_Validate_Then_ShouldReturnMultipleErrors()
    {
        // Arrange
        var model = new CreateMenuRequest
        (Guid.NewGuid(), "", -5m, null, MenuCategory.Dinner, null);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3);
    }
}