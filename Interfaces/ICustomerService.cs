using backend.Dto;

namespace backend.Interfaces
{
    public interface ICustomerService
    {
        int Register(RegisterUserDto dto);
        UserDto? Login(string email, string password);
        UserDto GetProfile(int id);
        IEnumerable<UserDto> GetAllCustomers();
        void UpdateProfile(int id, RegisterUserDto dto);
        Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync();
        void DeleteCustomer(int id);
    }
}
