using backend.Model;

namespace backend.Dto
{
    public class LoyaltyResult
    {
        public LoyaltyTier Tier { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsEligible => Tier != LoyaltyTier.None;
    }
}