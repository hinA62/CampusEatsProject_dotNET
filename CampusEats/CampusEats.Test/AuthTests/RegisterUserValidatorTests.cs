using CampusEats.Features.Auth.Requests;
using CampusEats.Validators.Auth;

namespace CampusEats.Test.AuthTests;

public class RegisterUserValidatorTests
{
    [Fact]
    public void Given_ValidInput_When_Validate_Then_ShouldPass()
    {
        // Arrange
        var model = new RegisterUserRequest
            ("Ion", "ion_lungu@mail.com", "SomeP@ssw0rd", "Client");
        var validator = new RegisterUserValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Given_EmptyUsername_When_Validate_Then_ShouldFail()
    {
        // Arrange
        var model = new RegisterUserRequest
            ("", "some-valid@mail.ro", "SomeP@ssw0rd", "Client");
        var validator = new RegisterUserValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Username is required.");
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    public void Given_ShortName_When_Validate_ShouldFail(string username)
    {
        // Arrange
        var model = new RegisterUserRequest
            (username, "some-valid@mail.com", "SomeP@ssw0rd", "Client");
        var validator = new RegisterUserValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Username must be at least 3 characters long.");
    }

    [Fact]
    public void Given_TooLongUsername_When_Validate_ShouldFail()
    {
        // Arrange
        var longUsername = new string('a', 51);
        var model = new RegisterUserRequest
            (longUsername, "some-valid@mail.com", "SomeP@ssw0rd", "Client");
        var validator = new RegisterUserValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Username cannot exceed 50 characters long.");
    }

    [Fact]
    public void Given_EmptyEmail_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new RegisterUserRequest
            ("Ion", "", "SomeP@ssw0rd", "Client");
        var validator = new RegisterUserValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Email is required.");
    }

    [Theory]
    [InlineData("invalid-email.com")]
    [InlineData("invalid@mail")]
    [InlineData("invalid-email")]
    public void Given_InvalidEmail_When_Validate_ShouldFail(string email)
    {
        // Arrange
        var model = new RegisterUserRequest
            ("Ion", email, "SomeP@ssw0rd", "Client");
        var validator = new RegisterUserValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == "Invalid email format.");
    }

    [Fact]
    public void Given_TooLongEmail_When_Validate_ShouldFail()
    {
        // Arrange
        var longEmail = new string('a', 101) + "@mail.com";
        var model = new RegisterUserRequest
            ("Ion", longEmail, "SomeP@ssw0rd", "Client");
        var validator = new RegisterUserValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == "Email cannot exceed 100 characters.");
    }

    [Fact]
    public void Given_EmptyPassword_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new RegisterUserRequest
            ("Ion", "some-valid@mail.com", "", "Client");
        var validator = new RegisterUserValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
           e.ErrorMessage == "Password is required.");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("123")]
    [InlineData("1234")]
    [InlineData("12345")]
    [InlineData("123456")]
    [InlineData("1234567")]
    [InlineData("12345678")]
    [InlineData("123456789")]
    public void Given_TooShortPassword_When_Validate_ShouldFail(string password)
    {
        // Arrange
        var model = new RegisterUserRequest
            ("Ion", "ion_lungu@mail.com", password, "Client");
        var validator = new RegisterUserValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage.Contains("Password must be at least 10 characters long."));
    }

    [Theory]
    [InlineData("1234567890!")]
    [InlineData("my_password")]
    [InlineData("My_password!")]
    [InlineData("password123")]
    [InlineData("Password123")]
    public void Given_InvalidPassword_When_Validate_ShouldFail(string password)
    {
        // Arrange
        var model = new RegisterUserRequest
            ("Ion", "ion_lungu@mail.com", password, "Client");
        var validator = new RegisterUserValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage.Contains("Password must contain at least"));
    }

    [Fact]
    public void Given_EmptyRole_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new RegisterUserRequest
            ("Ion", "ion_lungu@mail.com", "SomeP@ssw0rd", "");
        var validator = new RegisterUserValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == "Role is required.");
    }

    [Theory]
    [InlineData("User")]
    [InlineData("SuperAdmin")]
    [InlineData("Guest")]
    public void Given_InvalidRole_When_Validate_ShouldFail(string role)
    {
        // Arrange
        var model = new RegisterUserRequest
            ("Ion", "ion_lungu@mail.com", "SomeP@ssw0rd", role);
        var validator = new RegisterUserValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == "Invalid role. Valid roles: Client, Kitchen, Admin.");
    }
}