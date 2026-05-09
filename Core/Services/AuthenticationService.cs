using Domain.Entities.IdentitiyModule;
using Microsoft.AspNetCore.Identity;
using Services.Abstraction;
using Shared.Dtos.IdentityDtos;
using Domain.Exceptions;
namespace Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationService(UserManager<ApplicationUser> userManager)
        {
           _userManager = userManager;
        }
        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
                throw new Exception(message: "User InValid");

            var IsPasswordValid = await _userManager.CheckPasswordAsync(user , loginDto.Password);
            if (!IsPasswordValid) 
                throw new Exception(message: "User InValid");

            return new UserDto(user.Email!,user.DisplayName,"");
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = new ApplicationUser() { 
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber,
            };
            var IdentityResult = await _userManager.CreateAsync(user, registerDto.Password);
            if (!IdentityResult.Succeeded)
            {
                var errors = IdentityResult.Errors.Select(e => e.Description).ToList();
                throw new ValidationException(errors);
            }

            return new UserDto(user.Email, user.DisplayName, "");
        }
    }
}
