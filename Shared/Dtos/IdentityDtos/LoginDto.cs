using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.IdentityDtos
{
    public record LoginDto([EmailAddress]string Email, string Password);
   
}
