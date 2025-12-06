using CampusEats.Features.Payment.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Payment.Handlers;

public class GetPaymentByIdHandler
{
    private readonly CampusEatsContext _db;

    public GetPaymentByIdHandler(CampusEatsContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(GetPaymentByIdRequest request, CancellationToken ct = default)
    {
        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.Id == request.PaymentId, ct);
        return payment is null ? Results.NotFound() : Results.Ok(payment);
    }
}