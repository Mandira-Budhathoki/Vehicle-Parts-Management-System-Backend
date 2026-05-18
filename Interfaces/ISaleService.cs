using backend.Dto;
using backend.Model;

namespace backend.Interfaces
{
    public interface ISalesService
    {
        Task<Sales> CreateSaleAsync(CreateSalesDto dto);
    }
}