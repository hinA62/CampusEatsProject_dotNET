using CampusEats.Features.Payment.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Payment.Handlers;

public class GetPaymentHistoryHandler
{
    private readonly CampusEatsContext _db;

    public GetPaymentHistoryHandler(CampusEatsContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(GetPaymentHistoryRequest request, CancellationToken ct = default)
    {
        var payments = await _db.Payments
            .Where(p => p.UserId == request.UserId)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(ct);

        return Results.Ok(payments);
    }
}
