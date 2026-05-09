using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.IdentityDtos
{
    public record RegisterDto([EmailAddress] string Email,
                            string DisplayName,
                            string UserName,
                            string Password,
                            [Phone] string PhoneNumber);
  
}
