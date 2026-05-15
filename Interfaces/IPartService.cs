using backend.Dto;

namespace backend.Interfaces
{
    public interface IPartService
    {
        Task<IEnumerable<PartDto>> GetAllPartsAsync();
        Task<PartDto> GetPartByIdAsync(int id);
        Task<PartDto> CreatePartAsync(CreatePartDto createPartDto);
        Task<PartDto> UpdatePartAsync(int id, UpdatePartDto updatePartDto);
        Task<bool> DeletePartAsync(int id);
    }
}
