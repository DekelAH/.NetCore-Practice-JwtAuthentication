using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using System.Security.Claims;

namespace CitiesManager.Core.ServiceContracts
{
    public interface IJwtService
    {
        #region Methods

        AuthenticationResponse CreateJwtToken(ApplicationUser user);
        ClaimsPrincipal? GetClaimsPrincipalFromJwtToken(string? token);

        #endregion
    }
}
