using CampusEats.Features.Auth.Requests;
using CampusEats.Validators.Auth;

namespace CampusEats.Test.AuthTests;

public class ChangePasswordValidatorTests
{
    [Fact]
    public void Given_ValidInput_When_Validate_Then_ShouldPass()
    {
        // Arrange
        var model = new ChangePasswordRequest(Guid.NewGuid(), "OldP@ssw0rd123", "NewP@ssw0rd456");
        var validator = new ChangePasswordValidator();
        
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
        var model = new ChangePasswordRequest(Guid.Empty, "OldP@ssw0rd123", "NewP@ssw0rd456");
        var validator = new ChangePasswordValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "User ID is required");
    }
    
    [Fact]
    public void Given_EmptyCurrentPassword_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new ChangePasswordRequest(Guid.NewGuid(), "", "NewP@ssw0rd456");
        var validator = new ChangePasswordValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Current password is required");
    }
    
    [Fact]
    public void Given_EmptyNewPassword_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new ChangePasswordRequest(Guid.NewGuid(), "OldP@ssw0rd123", "");
        var validator = new ChangePasswordValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "New password is required");
    }
    
    [Theory]
    [InlineData("Short@1")]
    [InlineData("Tiny@1")]
    public void Given_ShortNewPassword_When_Validate_Then_ShouldFail(string password)
    {
        // Arrange
        var model = new ChangePasswordRequest(Guid.NewGuid(), "OldP@ssw0rd123", password);
        var validator = new ChangePasswordValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Password must be at least 10 characters long");
    }
    
    [Fact]
    public void Given_NewPasswordWithoutUppercase_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new ChangePasswordRequest(Guid.NewGuid(), "OldP@ssw0rd123", "newp@ssw0rd456");
        var validator = new ChangePasswordValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Password must contain at least one uppercase letter");
    }
    
    [Fact]
    public void Given_NewPasswordWithoutDigit_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new ChangePasswordRequest(Guid.NewGuid(), "OldP@ssw0rd123", "NewP@ssword");
        var validator = new ChangePasswordValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Password must contain at least one digit");
    }
    
    [Fact]
    public void Given_NewPasswordWithoutSpecialChar_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new ChangePasswordRequest(Guid.NewGuid(), "OldP@ssw0rd123", "NewPassword456");
        var validator = new ChangePasswordValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Password must contain at least one special character (@$!%*?&#)");
    }
    
    [Fact]
    public void Given_SameCurrentAndNewPassword_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new ChangePasswordRequest(Guid.NewGuid(), "SameP@ssw0rd123", "SameP@ssw0rd123");
        var validator = new ChangePasswordValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "New password must be different from current password");
    }
}
