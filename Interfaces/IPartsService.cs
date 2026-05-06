using backend.Dto;

namespace backend.Interfaces
{
    public interface IPartsService
    {
        Task<IEnumerable<PartDto>> GetAllPartsAsync();
        Task<PartDto?> GetPartByIdAsync(int id);
        Task<PartDto> CreatePartAsync(PartDto partDto);
        Task UpdatePartAsync(int id, PartDto partDto);
        Task DeletePartAsync(int id);
    }
}
