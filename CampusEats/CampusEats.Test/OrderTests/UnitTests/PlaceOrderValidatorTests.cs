using CampusEats.Features.Order.Requests;
using CampusEats.Validators.Order;

namespace CampusEats.Test.OrderTests.UnitTests;

public class PlaceOrderValidatorTests
{
    [Fact]
    public void Given_ValidInput_When_Validate_Then_ShouldPass()
    {
        // Arrange
        var model = new PlaceOrderRequest
            (Guid.NewGuid(), [Guid.NewGuid()], [Guid.NewGuid()]);
        var validator = new PlaceOrderValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    
    [Fact]
    public void Given_EmptyClientId_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new PlaceOrderRequest
            (Guid.Empty, [Guid.NewGuid()], []);
        var validator = new PlaceOrderValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == "Client Id is required.");
    }
    
    [Fact]
    public void Given_NullMenuId_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new PlaceOrderRequest
            (Guid.NewGuid(), null!, [Guid.NewGuid()]);
        var validator = new PlaceOrderValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == "At least one Menu must be provided.");
    }

    [Fact]
    public void Given_NullItemId_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new PlaceOrderRequest(Guid.NewGuid(), [Guid.NewGuid()], null!);
        var validator = new PlaceOrderValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "At least one Item must be provided.");
    }
    
    [Fact]
    public void Given_EmptyMenuOrItemIds_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new PlaceOrderRequest(Guid.NewGuid(), [], []);
        var validator = new PlaceOrderValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "At least one Menu or Item must be provided.");
    }
}