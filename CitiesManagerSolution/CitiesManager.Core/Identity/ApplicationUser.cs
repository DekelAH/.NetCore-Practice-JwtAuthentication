using Microsoft.AspNetCore.Identity;

namespace CitiesManager.Core.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        #region Properties

        public string? PersonName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDateTime { get; set; }

        #endregion
    }
}
