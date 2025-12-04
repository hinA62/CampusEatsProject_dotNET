using CampusEats.Features.Kitchen.Requests;
using CampusEats.Features.Order;
using CampusEats.Validators.Kitchen;

namespace CampusEats.Test.KitchenTests;

public class UpdateOrderStatusValidatorTests
{
    [Fact]
    public void Given_ValidInput_When_Validate_Then_ShouldPass()
    {
        // Arrange
        var model = new UpdateOrderStatusRequest(Guid.NewGuid(), OrderStatus.Preparing);
        var validator = new UpdateOrderStatusValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Given_EmptyOrderId_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new UpdateOrderStatusRequest(Guid.Empty, OrderStatus.Preparing);
        var validator = new UpdateOrderStatusValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.PropertyName == "OrderId" && e.ErrorMessage.Contains("OrderId is required"));
    }

    [Fact]
    public void Given_InvalidEnumStatus_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new UpdateOrderStatusRequest(Guid.NewGuid(), (OrderStatus)999);
        var validator = new UpdateOrderStatusValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.PropertyName == "NewStatus" && e.ErrorMessage.Contains("Invalid order status"));
    }

    [Fact]
    public void Given_CancelledStatusForNewOrder_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new UpdateOrderStatusRequest(Guid.NewGuid(), OrderStatus.Cancelled);
        var validator = new UpdateOrderStatusValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.PropertyName == "NewStatus" && e.ErrorMessage.Contains("Cannot change status to Cancelled"));
    }
    
    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Preparing)]
    [InlineData(OrderStatus.Completed)]
    public void Given_InvalidOrderStatus_When_Validate_Then_ShouldFail(OrderStatus status)
    {
        // Arrange
        var validStatuses = new[]
        {
            OrderStatus.Pending, 
            OrderStatus.Confirmed, 
            OrderStatus.Preparing, 
            OrderStatus.Completed
        };
        var validator = new UpdateOrderStatusValidator();
        
        // Act
        var model = new UpdateOrderStatusRequest(Guid.NewGuid(), status);
        var result = validator.Validate(model);

        // Assert
        if (validStatuses.Contains(status)) return;
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.PropertyName == "NewStatus" && e.ErrorMessage.Contains("Invalid order status"));
    }
    
    [Fact]
    public void Given_MultipleInvalidFields_When_Validate_Then_ShouldReturnMultipleErrors()
    {
        // Arrange
        var model = new UpdateOrderStatusRequest(Guid.Empty, OrderStatus.Cancelled);
        var validator = new UpdateOrderStatusValidator();
        
        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 2);
    }
}