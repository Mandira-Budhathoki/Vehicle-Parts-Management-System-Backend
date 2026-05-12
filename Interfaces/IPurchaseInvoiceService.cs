using backend.Dto;

namespace backend.Interfaces
{
    public interface IPurchaseInvoiceService
    {
        Task<IEnumerable<PurchaseInvoiceDto>> GetAllPurchaseInvoicesAsync();
        Task<PurchaseInvoiceDto> CreatePurchaseInvoiceAsync(int adminUserId, CreatePurchaseInvoiceDto dto);
        Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
        Task<IEnumerable<object>> GetAllPartsAsync(); // Using anonymous object or create a simple PartDto
    }
}
