using Domain.Entities.IdentitiyModule;
using Microsoft.AspNetCore.Identity;
using Services.Abstraction;
using Shared.Dtos.IdentityDtos;
using Domain.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared;
namespace Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IOptions<JwtOptions> _options;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IOptions<JwtOptions> options)
        {
           _userManager = userManager;
            _configuration = configuration;
            _options = options;
        }
        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
                throw new Exception(message: "User InValid");

            var IsPasswordValid = await _userManager.CheckPasswordAsync(user , loginDto.Password);
            if (!IsPasswordValid) 
                throw new Exception(message: "User InValid");

            var token = await CreateTokenAsync(user);

            return new UserDto(user.Email!,user.DisplayName, token);
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
            var token = await CreateTokenAsync(user);
            return new UserDto(user.Email, user.DisplayName, token);
        }
    
        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            // Token [Issure, Audience, Claims, ExpireDate, signingCredentials]

            var jwtOptions = _options.Value;
            // Create Claims
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.DisplayName),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Generate Credentials
            var secretKey = jwtOptions.SecretKey;
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var Cred = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            // Create Token
            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                expires: DateTime.UtcNow.AddDays(2),
                claims: claims,
                signingCredentials:Cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
