using CampusEats.Features.Auth.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Auth.Handlers;

public class ChangePasswordHandler(CampusEatsContext db)
{
    public async Task<IResult> Handle(ChangePasswordRequest request, CancellationToken ct = default)
    {
        // Găsim utilizatorul
        var user = await db.Users.FindAsync(new object[] { request.UserId }, ct);
        
        if (user is null)
            return Results.NotFound("User not found");

        // Verificăm parola curentă
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return Results.BadRequest("Current password is incorrect");

        // Actualizăm parola
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        
        await db.SaveChangesAsync(ct);

        return Results.Ok(new { message = "Password changed successfully" });
    }
}

