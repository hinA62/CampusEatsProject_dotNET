namespace CampusEats.Features.Auth.Requests;

public record UpdateUserRoleRequest(
    Guid UserId,
    string NewRole
);

