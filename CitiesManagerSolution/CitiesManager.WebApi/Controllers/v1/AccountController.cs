using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitiesManager.WebApi.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    public class AccountController : CustomControllerBase
    {
        #region Fields

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;

        #endregion

        #region Ctors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="roleManager"></param>
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Receiving registerDTO model to add to user data base
        /// </summary>
        /// <param name="registerDTO">RegisterDTO model to add</param>
        /// <returns>Registered users</returns>
        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(errors => errors.Errors)
                                                                          .Select(error => error.ErrorMessage));
                return Problem(errorMessage);
            }

            ApplicationUser applicationUser = new ApplicationUser()
            {
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                UserName = registerDTO.Email,
                PersonName = registerDTO.PersonName
            };
            IdentityResult result = await _userManager.CreateAsync(applicationUser, registerDTO.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(applicationUser, isPersistent: false);
                var authenticationResponse = _jwtService.CreateJwtToken(applicationUser);
                applicationUser.RefreshToken = authenticationResponse.RefreshToken;
                applicationUser.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
                await _userManager.UpdateAsync(applicationUser);

                return Ok(authenticationResponse);
            }
            else
            {
                string errorMessage = string.Join(" | ", result.Errors.Select(error => error.Description));
                return Problem(errorMessage);
            }
        }


        /// <summary>
        /// Receiving email address to check if it is already in use and returns a boolean response
        /// </summary>
        /// <param name="email">email address to check</param>
        /// <returns>False if not exist, True if exist</returns>
        [HttpGet]
        public async Task<IActionResult> IsIsEmailAlreadyRegister(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> PostLogin(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(errors => errors.Errors)
                                                                          .Select(error => error.ErrorMessage));
                return Problem(errorMessage);
            }

            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password,
                                                                    isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (applicationUser == null)
                {
                    return NoContent();
                }

                await _signInManager.SignInAsync(applicationUser, isPersistent: false);
                var authenticationResponse = _jwtService.CreateJwtToken(applicationUser);
                applicationUser.RefreshToken = authenticationResponse.RefreshToken;
                applicationUser.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
                await _userManager.UpdateAsync(applicationUser);

                return Ok(authenticationResponse);
            }
            else
            {
                return Problem("Invalid Email and Password");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        [HttpPost("generate-new-jwt-token")]
        public async Task<IActionResult> GenerateNewAccessToken(TokenModel tokenModel)
        {
            if (tokenModel == null)
            {
                return BadRequest("Invalid user request");
            }

            ClaimsPrincipal? claimsPrincipal = _jwtService.GetClaimsPrincipalFromJwtToken(tokenModel.Token);

            if (claimsPrincipal == null)
            {
                return BadRequest("Invalid access token");
            }

            string? email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(email);

            if (applicationUser == null || applicationUser.RefreshToken != tokenModel.RefreshToken ||
                applicationUser.RefreshTokenExpirationDateTime <= DateTime.Now)
            {
                return BadRequest();
            }

            AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(applicationUser);

            applicationUser.RefreshToken = authenticationResponse.RefreshToken;
            applicationUser.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;

            await _userManager.UpdateAsync(applicationUser);

            return Ok(authenticationResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public async Task<ActionResult<ApplicationUser>> GetLogout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }

        #endregion
    }
}
