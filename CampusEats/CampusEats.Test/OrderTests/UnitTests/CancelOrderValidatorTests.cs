using CampusEats.Features.Order.Requests;
using CampusEats.Validators.Order;

namespace CampusEats.Test.OrderTests.UnitTests;

public class CancelOrderValidatorTests
{
    [Fact]
    public void Given_ValidCancelOrderId_When_Validate_ShouldPass()
    {
        // Arrange
        var model = new CancelOrderRequest(Guid.NewGuid());
        var validator = new CancelOrderValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    
    [Fact]
    public void Given_EmptyCancelOrderId_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new CancelOrderRequest(Guid.Empty);
        var validator = new CancelOrderValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage.Contains("Order Id is required."));
    }
}