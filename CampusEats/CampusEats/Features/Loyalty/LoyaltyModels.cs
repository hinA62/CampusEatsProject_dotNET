namespace CampusEats.Features.Loyalty;

public enum LoyaltyTransactionType
{
    Earn,
    Redeem,
    CashbackBonus
}

public enum LoyaltyTier
{
    Bronze,   // 0-99 points
    Silver,   // 100-499 points
    Gold,     // 500-1499 points
    Platinum, // 1500-4999 points
    VIP       // 5000+ points
}

public class LoyaltyAccount
{
    public Guid UserId { get; set; }

    public int Points { get; set; }
    public int TotalPointsEarned { get; set; } // Pentru tier calculation
    public LoyaltyTier CurrentTier { get; set; }
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

public static class LoyaltyTierHelper
{
    // Conversion rate: 100 points = $1 (for redemption only)
    public const int POINTS_PER_DOLLAR = 100;
    
    // Tier thresholds based on total points earned
    private static readonly Dictionary<LoyaltyTier, int> TierThresholds = new()
    {
        { LoyaltyTier.Bronze, 0 },
        { LoyaltyTier.Silver, 100 },     // $100 spent
        { LoyaltyTier.Gold, 500 },       // $500 spent
        { LoyaltyTier.Platinum, 1500 },  // $1500 spent
        { LoyaltyTier.VIP, 5000 }        // $5000 spent
    };

    // Cashback rates: points earned per dollar spent
    private static readonly Dictionary<LoyaltyTier, decimal> TierCashbackRates = new()
    {
        { LoyaltyTier.Bronze, 1.00m },    // 1 point per $1
        { LoyaltyTier.Silver, 3.00m },    // 3 points per $1
        { LoyaltyTier.Gold, 5.00m },      // 5 points per $1
        { LoyaltyTier.Platinum, 8.00m },  // 8 points per $1
        { LoyaltyTier.VIP, 12.00m }       // 12 points per $1
    };

    public static LoyaltyTier CalculateTier(int totalPointsEarned)
    {
        if (totalPointsEarned >= TierThresholds[LoyaltyTier.VIP])
            return LoyaltyTier.VIP;
        if (totalPointsEarned >= TierThresholds[LoyaltyTier.Platinum])
            return LoyaltyTier.Platinum;
        if (totalPointsEarned >= TierThresholds[LoyaltyTier.Gold])
            return LoyaltyTier.Gold;
        if (totalPointsEarned >= TierThresholds[LoyaltyTier.Silver])
            return LoyaltyTier.Silver;
        
        return LoyaltyTier.Bronze;
    }

    public static decimal GetCashbackRate(LoyaltyTier tier)
    {
        return TierCashbackRates.GetValueOrDefault(tier, 1.00m);
    }

    public static int GetTierThreshold(LoyaltyTier tier)
    {
        return TierThresholds.GetValueOrDefault(tier, 0);
    }

    public static LoyaltyTier? GetNextTier(LoyaltyTier currentTier)
    {
        return currentTier switch
        {
            LoyaltyTier.Bronze => LoyaltyTier.Silver,
            LoyaltyTier.Silver => LoyaltyTier.Gold,
            LoyaltyTier.Gold => LoyaltyTier.Platinum,
            LoyaltyTier.Platinum => LoyaltyTier.VIP,
            LoyaltyTier.VIP => null,
            _ => null
        };
    }

    public static int GetPointsToNextTier(int totalPointsEarned, LoyaltyTier currentTier)
    {
        var nextTier = GetNextTier(currentTier);
        if (nextTier == null) return 0;
        
        return GetTierThreshold(nextTier.Value) - totalPointsEarned;
    }
}