using System.Runtime.InteropServices;
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
            e.PropertyName == "Username" && e.ErrorMessage.Contains(" must not be empty."));
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    public void Given_InvalidName_When_Validate_ShouldFail(string username)
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
            e.PropertyName == "Username" && 
            e.ErrorMessage.Contains(" must be at least 3 characters long and maximum 50 characters long."));
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
            e.PropertyName == "Email" && e.ErrorMessage.Contains(" must not be empty."));
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
            e.PropertyName == "Email" && e.ErrorMessage.Contains(" is not a valid e-mail address."));
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
            e.PropertyName == "Email" && e.ErrorMessage.Contains(" must be at most 100 characters long."));
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
            e.PropertyName == "Password" && e.ErrorMessage.Contains(" must not be empty."));
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
        var model = new RegisterUserRequest("Ion", "ion_lungu@mail.com", password, "Client");
        var validator = new RegisterUserValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.PropertyName == "Password" && e.ErrorMessage.Contains(" must be at least 10 characters long."));
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
        var model = new RegisterUserRequest("Ion", "ion_lungu@mail.com", password, "Client");
        var validator = new RegisterUserValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.PropertyName == "Password" &&
            e.ErrorMessage.Contains(" must contain at least one uppercase letter," +
                                    " one lowercase letter, one digit and one special character."));
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
            e.PropertyName == "Role" && e.ErrorMessage.Contains(" must not be empty."));
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
            e.PropertyName == "Role" && e.ErrorMessage.Contains(" is not a valid role."));
    }

    [Fact]
    public void Given_MultipleInvalidData_When_Validate_ShouldFail()
    {
        // Arrange
        var model = new RegisterUserRequest
            ("", "invalid-email", "123", "InvalidRole");
        var validator = new RegisterUserValidator();
        
        // Act
        var result = validator.Validate(model);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
    }
}