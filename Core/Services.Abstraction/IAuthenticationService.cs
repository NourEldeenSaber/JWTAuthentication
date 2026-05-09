using Shared.Dtos.IdentityDtos;

namespace Services.Abstraction
{
    public interface IAuthenticationService
    {
        // Login    { Email, Password  =>> Token, DisplayName, Email }
        Task<UserDto> LoginAsync(LoginDto loginDto);

        // Register { Email, Password, UserName, DisplayName, PhoneNumber  =>> Token, DisplayName, Email }
        Task<UserDto> RegisterAsync(RegisterDto registerDto);
    }
}
