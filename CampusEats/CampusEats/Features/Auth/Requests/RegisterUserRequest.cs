namespace CampusEats.Features.Auth.Requests;

public record RegisterUserRequest(
    string Username,
    string Email,
    string Password,
    string ConfirmPassword
);