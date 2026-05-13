using backend.Dto;

namespace backend.Interfaces
{
    public interface ICustomerService
    {
        int Register(RegisterUserDto dto);
        UserDto? Login(string email, string password);
        UserDto GetProfile(int id);
        void UpdateProfile(int id, RegisterUserDto dto);
    }
}
