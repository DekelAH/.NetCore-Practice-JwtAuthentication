using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.DTO
{
    public class LoginDTO
    {
        #region Properties

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should contain a proper email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password can't be blank")]
        public string Password { get; set; } = string.Empty;

        #endregion
    }
}
