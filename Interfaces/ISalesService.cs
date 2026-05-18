using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Dto;
using backend.Model;

namespace backend.Interfaces
{
    public interface ISalesService
    {
        Task<SalesDto> CreateSalesInvoiceAsync(CreateSalesDto dto);
        Task<IEnumerable<SalesDto>> GetAllSalesInvoicesAsync();
        Task<SalesDto> GetSalesInvoiceByIdAsync(int id);
        //Task<Sales> CreateSaleAsync(CreateSalesDto dto);
    }
}
