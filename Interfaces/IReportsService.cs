using backend.Dto;

namespace backend.Interfaces
{
    public interface IReportsService
    {
        Task<IEnumerable<RegularCustomerDto>> GetRegularCustomersAsync();
        Task<IEnumerable<HighSpenderDto>> GetHighSpendersAsync();
        Task<IEnumerable<PendingCreditDto>> GetPendingCreditsAsync();
    }
}