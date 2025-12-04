namespace CampusEats.Features.Auth.Requests;

public record ChangePasswordRequest(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
);

