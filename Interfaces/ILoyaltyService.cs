using backend.Dto;

namespace backend.Interfaces
{
    public interface ILoyaltyService
    {
        LoyaltyResult CalculateLoyalty(decimal totalSpent);
    }
}