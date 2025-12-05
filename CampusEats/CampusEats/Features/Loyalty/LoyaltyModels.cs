namespace CampusEats.Features.Loyalty;

public enum LoyaltyTransactionType
{
    Earn,
    Redeem
}

public class LoyaltyAccount
{
    public Guid UserId { get; set; }

    public int Points { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public ICollection<LoyaltyTransaction> Transactions { get; set; } = new List<LoyaltyTransaction>();
}

public class LoyaltyTransaction
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public LoyaltyTransactionType Type { get; set; }
    public int Points { get; set; }
    public string? Description { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
