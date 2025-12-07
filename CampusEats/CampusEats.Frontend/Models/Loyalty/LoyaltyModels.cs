namespace CampusEatsFrontend.Models.Loyalty;

public enum LoyaltyTransactionType
{
    Earn,
    Redeem,
    CashbackBonus
}

public enum LoyaltyTier
{
    Bronze,
    Silver,
    Gold,
    Platinum,
    VIP
}

public class LoyaltyBalanceDto
{
    public Guid UserId { get; set; }
    public int Points { get; set; }
    public int TotalPointsEarned { get; set; }
    public LoyaltyTier CurrentTier { get; set; }
    public decimal CashbackRate { get; set; }
    public LoyaltyTier? NextTier { get; set; }
    public int PointsToNextTier { get; set; }
}

public class LoyaltyTransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public LoyaltyTransactionType Type { get; set; }
    public int Points { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class RedeemPointsRequest
{
    public Guid UserId { get; set; }
    public int PointsToRedeem { get; set; }
}
