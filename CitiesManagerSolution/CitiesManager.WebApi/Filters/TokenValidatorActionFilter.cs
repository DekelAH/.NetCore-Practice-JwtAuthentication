using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.WebApi.Controllers.v1;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CitiesManager.WebApi.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenValidatorActionFilter : IAsyncActionFilter
    {

        #region Fields

        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;

        #endregion

        #region Ctors
        /// <summary>
        /// 
        /// </summary>
        public TokenValidatorActionFilter(IJwtService jwtService, UserManager<ApplicationUser> userManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var authorizationHeader = context.HttpContext.Request.Headers.Authorization;
            var acceptHeader = context.HttpContext.Request.Headers.Accept;
            var actionName = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;

            if (!string.IsNullOrEmpty(authorizationHeader) && !string.IsNullOrEmpty(acceptHeader))
            {
                if (context.Controller is CitiesController citiesController)
                {
                    var token = authorizationHeader.ToString().Replace("Bearer ", string.Empty);
                    var refreshToken = acceptHeader.ToString();

                    var jwtToCheck = new JwtSecurityToken(token);
                    if (jwtToCheck.ValidTo <= DateTime.UtcNow)
                    {

                        ClaimsPrincipal? claimsPrincipal = _jwtService.GetClaimsPrincipalFromJwtToken(token);
                        if (claimsPrincipal == null)
                        {
                            context.Result = citiesController.Unauthorized("Invalid Access");
                            return;
                        }

                        string? email = claimsPrincipal?.FindFirstValue(ClaimTypes.Email);
                        ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(email);

                        if (applicationUser == null || applicationUser.RefreshToken != refreshToken ||
                        applicationUser.RefreshTokenExpirationDateTime <= DateTime.Now)
                        {
                            context.Result = citiesController.Unauthorized("Invalid Access, login time-out");
                            return;
                        }
                        AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(applicationUser);

                        applicationUser.RefreshToken = authenticationResponse.RefreshToken;
                        applicationUser.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;

                        await _userManager.UpdateAsync(applicationUser);

                        citiesController.Ok(authenticationResponse);
                    }
                }
            }
            else
            {
                context.Result = new StatusCodeResult(400);
            }
            await next();
        }

        #endregion
    }
}
