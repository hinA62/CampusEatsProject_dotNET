using CampusEats.Features.Menu.Requests;
using CampusEats.Validators.Menu;

namespace CampusEats.Test;

public class CreateItemValidatorTests
{
    [Fact]
    public void Given_ValidInput_When_Validate_Then_ShouldPass()
    {
        var model = new CreateItemRequest(Guid.NewGuid(), "Valid Item Name", 10.99m, "https://example.com/image.jpg", null);
        var validator = new CreateItemValidator();

        var result = validator.Validate(model);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Given_NullOrEmptyName_When_Validate_Then_ShouldFail(string name)
    {
        var model = new CreateItemRequest(Guid.NewGuid(), name, 10.12m, "https://example.com/image.jpg", null);
        var validator = new CreateItemValidator();

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    public void Given_NameLessThan3Characters_When_Validate_Then_ShouldFail(string name)
    {
        var model = new CreateItemRequest(Guid.NewGuid(), name, 10.99m, "https://example.com/image.jpg", null);
        var validator = new CreateItemValidator();

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage.Contains("at least 3 characters"));
    }

    [Fact]
    public void Given_NullPrice_When_Validate_Then_ShouldFail()
    {
        var model = new CreateItemRequest(Guid.NewGuid(), "Valid Item", null, "https://example.com/image.jpg", null);
        var validator = new CreateItemValidator();

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.99)]
    public void Given_ZeroOrNegativePrice_When_Validate_Then_ShouldFail(decimal price)
    {
        var model = new CreateItemRequest(Guid.NewGuid(), "Valid Item", price, "https://example.com/image.jpg", null);
        var validator = new CreateItemValidator();

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price" && e.ErrorMessage.Contains("greater than 0"));
    }

    [Fact]
    public void Given_MultipleInvalidFields_When_Validate_Then_ShouldReturnMultipleErrors()
    {
        var model = new CreateItemRequest(Guid.Empty, "ab", -5m, "invalid-url", null);
        var validator = new CreateItemValidator();

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3);
    }
}