using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared.Dtos.IdentityDtos;

namespace Presentation
{
    public class AuthenticationController : ApiController
    {
        private readonly IAuthenticationService _service;

        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        // Login     Post : BaseUrl/api/Authentication/Login
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> LoginAsync([FromBody] LoginDto loginDto)
            => new OkObjectResult(await _service.LoginAsync(loginDto));

        // Register  Post : BaseUrl/api/Authentication/Register
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> RegisterAsync(RegisterDto registerDto)
            => new OkObjectResult(await _service.RegisterAsync(registerDto));
    }
}
