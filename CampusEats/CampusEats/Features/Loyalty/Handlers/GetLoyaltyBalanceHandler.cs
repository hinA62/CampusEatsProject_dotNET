using CampusEats.Features.Loyalty.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Loyalty.Handlers;

public class GetLoyaltyBalanceHandler
{
    private readonly CampusEatsContext _db;

    public GetLoyaltyBalanceHandler(CampusEatsContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(GetLoyaltyBalanceRequest request, CancellationToken ct = default)
    {
        var account = await _db.LoyaltyAccounts.FirstOrDefaultAsync(a => a.UserId == request.UserId, ct);
        var points = account?.Points ?? 0;
        return Results.Ok(new { userId = request.UserId, points });
    }
}
