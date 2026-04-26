using backend.Dto;

namespace backend.Interfaces
{
    public interface ICustomerService
    {
        void Register(RegisterUserDto dto);
        UserDto GetProfile(int id);
        void UpdateProfile(int id, RegisterUserDto dto);
    }
}
