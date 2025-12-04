using CampusEats.Features.Payment;
using CampusEats.Features.Payment.Requests;
using CampusEats.Validators.Payment;

namespace CampusEats.Test.PaymentTests;

public class CreatePaymentValidatorTests
{
    [Fact]
    public void Given_ValidInput_When_Validate_Then_ShouldPass()
    {
        // Arrange
        var model = new CreatePaymentRequest
            (Guid.NewGuid(), Guid.NewGuid(),
                13.75m, PaymentMethod.MockCard);
        var validator = new CreatePaymentValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    
    [Fact]
    public void Given_EmptyUserId_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new CreatePaymentRequest
            (Guid.Empty, Guid.NewGuid(),
                13.75m, PaymentMethod.MockCard);
        var validator = new CreatePaymentValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "UserId is required.");
    }

    [Fact]
    public void Given_EmptyOrderId_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new CreatePaymentRequest
            (Guid.NewGuid(), Guid.Empty,
                 13.75m, PaymentMethod.MockCard);
        var validator = new CreatePaymentValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "OrderId is required.");
    }

    [Fact]
    public void Given_NegativeAmount_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new CreatePaymentRequest
            (Guid.NewGuid(), Guid.NewGuid(),
                 -13.75m, PaymentMethod.MockCard);
        var validator = new CreatePaymentValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Amount must be greater than 0.");
    }

    [Fact]
    public void Given_MultipleInvalid_Input_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new CreatePaymentRequest
            (Guid.Empty, Guid.Empty,
                 -13.75m, PaymentMethod.MockCard);
        var validator = new CreatePaymentValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);
    }
}