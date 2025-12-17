using CampusEats.Features.Auth.Requests;
using CampusEats.Validators.Auth;

namespace CampusEats.Test.AuthTests;

public class UpdateUserRoleValidatorTests
{
    [Theory]
    [InlineData("Client")]
    [InlineData("Kitchen")]
    [InlineData("Admin")]
    public void Given_ValidRole_When_Validate_Then_ShouldPass(string role)
    {
        // Arrange
        var model = new UpdateUserRoleRequest(Guid.NewGuid(), role);
        var validator = new UpdateUserRoleValidator();
        
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
        var model = new UpdateUserRoleRequest(Guid.Empty, "Client");
        var validator = new UpdateUserRoleValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "User ID is required.");
    }
    
    [Fact]
    public void Given_EmptyRole_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new UpdateUserRoleRequest(Guid.NewGuid(), "");
        var validator = new UpdateUserRoleValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Role is required.");
    }
    
    [Theory]
    [InlineData("InvalidRole")]
    [InlineData("User")]
    [InlineData("Manager")]
    public void Given_InvalidRole_When_Validate_Then_ShouldFail(string role)
    {
        // Arrange
        var model = new UpdateUserRoleRequest(Guid.NewGuid(), role);
        var validator = new UpdateUserRoleValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Invalid role. Valid roles: Client, Kitchen, Admin.");
    }
    
    [Theory]
    [InlineData("client")]
    [InlineData("kitchen")]
    [InlineData("admin")]
    public void Given_LowercaseValidRole_When_Validate_Then_ShouldPass(string role)
    {
        // Arrange
        var model = new UpdateUserRoleRequest(Guid.NewGuid(), role);
        var validator = new UpdateUserRoleValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
