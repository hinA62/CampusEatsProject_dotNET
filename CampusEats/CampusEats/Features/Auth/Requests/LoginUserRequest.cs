namespace CampusEats.Features.Auth.Requests;

public record LoginUserRequest(
    string Email,
    string Password
);
