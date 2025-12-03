using CampusEats.Features.Menu.Requests;
using CampusEats.Validators.Menu;

namespace CampusEats.Test.MenuTests;

public class CreateItemValidatorTests
{
    [Fact]
    public void Given_ValidInput_When_Validate_Then_ShouldPass()
    {
        // Arrange
        var model = new CreateItemRequest
            (Guid.NewGuid(), "Valid Item Name", 10.99m, "https://example.com/image.jpg", null);
        var validator = new CreateItemValidator();

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
        var model = new CreateItemRequest
            (Guid.NewGuid(), name, 10.12m, "https://example.com/image.jpg", null);
        var validator = new CreateItemValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.PropertyName == "Name" && e.ErrorMessage.Contains("required"));
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    public void Given_NameLessThan3Characters_When_Validate_Then_ShouldFail(string name)
    {
        // Arrange
        var model = new CreateItemRequest
            (Guid.NewGuid(), name, 10.99m, "https://example.com/image.jpg", null);
        var validator = new CreateItemValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == "Name" && e.ErrorMessage.Contains("at least 3 characters"));
    }

    [Fact]
    public void Given_NullPrice_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new CreateItemRequest
            (Guid.NewGuid(), "Valid Item", null, "https://example.com/image.jpg", null);
        var validator = new CreateItemValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.PropertyName == "Price" && e.ErrorMessage.Contains("required"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.99)]
    public void Given_ZeroOrNegativePrice_When_Validate_Then_ShouldFail(decimal price)
    {
        // Arrange
        var model = new CreateItemRequest
            (Guid.NewGuid(), "Valid Item", price, "https://example.com/image.jpg", null);
        var validator = new CreateItemValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.PropertyName == "Price" && e.ErrorMessage.Contains("must be greater than 0"));
    }

    [Fact]
    public void Given_MultipleInvalidFields_When_Validate_Then_ShouldReturnMultipleErrors()
    {
        // Arrange
        var model = new CreateItemRequest(Guid.Empty, "ab", -5m, "invalid-url", null);
        var validator = new CreateItemValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3);
    }
}