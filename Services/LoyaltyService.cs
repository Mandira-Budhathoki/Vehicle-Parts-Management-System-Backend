using backend.Dto;
using backend.Interfaces;
using backend.Model;

namespace backend.Services
{
    public class LoyaltyService : ILoyaltyService
    {
        public LoyaltyResult CalculateLoyalty(decimal totalSpent)
        {
            if (totalSpent >= 10000)
            {
                return new LoyaltyResult
                {
                    Tier = LoyaltyTier.Gold,
                    DiscountPercentage = 15
                };
            }

            if (totalSpent >= 5000)
            {
                return new LoyaltyResult
                {
                    Tier = LoyaltyTier.Silver,
                    DiscountPercentage = 10
                };
            }

            if (totalSpent >= 2000)
            {
                return new LoyaltyResult
                {
                    Tier = LoyaltyTier.Bronze,
                    DiscountPercentage = 5
                };
            }

            return new LoyaltyResult
            {
                Tier = LoyaltyTier.None,
                DiscountPercentage = 0
            };
        }
    }
}