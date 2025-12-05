using CampusEats.Features.Menu;
using CampusEats.Features.Menu.Requests;
using CampusEats.Validators.Menu;

namespace CampusEats.Test.MenuTests;

public class UpdateMenuValidatorTests
{
    [Fact]
    public void Given_ValidInput_When_Validate_Then_ShouldPass()
    {
        // Arrange
        var model = new UpdateMenuRequest
        (Guid.NewGuid(), "Valid Menu Name", 15.99m, 
            [Guid.NewGuid()], MenuCategory.Breakfast);
        var validator = new UpdateMenuValidator();

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
        var model = new UpdateMenuRequest
            (Guid.NewGuid(), name, 15.99m,
                [Guid.NewGuid()], MenuCategory.Vegetarian);
        var validator = new UpdateMenuValidator();

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
        var model = new UpdateMenuRequest
            (Guid.NewGuid(), longName, 15.99m, 
                [Guid.NewGuid()], MenuCategory.Traditional);
        var validator = new UpdateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == "Menu name must not exceed 50 characters.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.99)]
    public void Given_ZeroOrNegativePrice_When_Validate_Then_ShouldFail(decimal price)
    {
        // Arrange
        var model = new UpdateMenuRequest
        (Guid.NewGuid(), "Valid Menu", price,
            [Guid.NewGuid()], MenuCategory.Vegan);
        var validator = new UpdateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Price must be greater than zero when provided.");
    }
    
    [Theory]
    [InlineData(null)]
    public void Given_NullOrEmptyItemIds_When_Validate_Then_ShouldFail(List<Guid> itemIds)
    {
        // Arrange
        var model = new UpdateMenuRequest
        ( Guid.NewGuid(), "Valid Menu", 15.99m, 
            itemIds, MenuCategory.Dessert);
        var validator = new UpdateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == "At least one menu item is required.");
    }

    [Fact]
    public void Given_MultipleInvalidFields_When_Validate_Then_ShouldReturnMultipleErrors()
    {
        // Arrange
        var model = new UpdateMenuRequest
            (Guid.NewGuid(),"", -5m, null, MenuCategory.Dinner);
        var validator = new UpdateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3);
    }
}