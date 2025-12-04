using CampusEats.Features.Loyalty.Requests;
using CampusEats.Validators.Loyalty;

namespace CampusEats.Test.LoyaltyTests;

public class RedeemPointsValidatorTests
{
    [Fact]
    public void Given_ValidInput_When_Validate_Then_ShouldPass()
    {
        // Arrange
        var model = new RedeemPointsRequest(Guid.NewGuid(), 10);
        var validator = new RedeemPointsValidator();

        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Given_EmptyGuid_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new RedeemPointsRequest(Guid.Empty, 10);
        var validator = new RedeemPointsValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage.Contains("User Id is required"));
    }

    [Fact]
    public void Given_NegativePoints_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new RedeemPointsRequest(Guid.NewGuid(), -1);
        var validator = new RedeemPointsValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Points to redeem must be greater than 0.");
    }
}