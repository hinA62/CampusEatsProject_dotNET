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
        var model = new CreateMenuRequest( "Valid Menu Name", 15.99m, [Guid.NewGuid()], MenuCategory.Breakfast, DietaryRestrictions.GlutenFree);
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
        var model = new CreateMenuRequest(name, 15.99m, [Guid.NewGuid()], MenuCategory.Vegetarian, DietaryRestrictions.None);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Given_NameExceeds50Characters_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var longName = new string('a', 51);
        var model = new CreateMenuRequest(longName, 15.99m, [Guid.NewGuid()], MenuCategory.Traditional, DietaryRestrictions.None);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage.Contains("cannot exceed 50 characters"));
    }

    [Fact]
    public void Given_NullPrice_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new CreateMenuRequest("Valid Menu", null, [Guid.NewGuid()], MenuCategory.Asian, DietaryRestrictions.LactoseFree);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.99)]
    public void Given_ZeroOrNegativePrice_When_Validate_Then_ShouldFail(decimal price)
    {
        // Arrange
        var model = new CreateMenuRequest("Valid Menu", price, [Guid.NewGuid()], MenuCategory.Vegan, DietaryRestrictions.SugarFree);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price" && e.ErrorMessage.Contains("greater than zero"));
    }

    [Theory]
    [MemberData(nameof(GetNullOrEmptyItemIds))]
    public void Given_NullOrEmptyItemIds_When_Validate_Then_ShouldFail(List<Guid> itemIds)
    {
        // Arrange
        var model = new CreateMenuRequest( "Valid Menu", 15.99m, itemIds, MenuCategory.Dessert, DietaryRestrictions.None);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ItemIds");
    }

    [Fact]
    public void Given_MultipleInvalidFields_When_Validate_Then_ShouldReturnMultipleErrors()
    {
        // Arrange
        var model = new CreateMenuRequest("", -5m, null, MenuCategory.Dinner, DietaryRestrictions.FoodAllergyFriendly);
        var validator = new CreateMenuValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3);
    }

    public static IEnumerable<object?[]> GetNullOrEmptyItemIds()
    {
        yield return [null];
        yield return [new List<Guid>()];
    }
}