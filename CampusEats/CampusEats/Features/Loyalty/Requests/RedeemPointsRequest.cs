namespace CampusEats.Features.Loyalty.Requests;

public record RedeemPointsRequest(
    Guid UserId,
    int PointsToRedeem);
