using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CitiesManager.Core.Services
{
    public class JwtService : IJwtService
    {

        #region Fields

        private IConfiguration _configuration;

        #endregion

        #region Ctors

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates a JWT token using the given user's information and the configuration settings
        /// </summary>
        /// <param name="user">ApplicationUser object</param>
        /// <returns>AuthenticationResponse that includes token</returns>
        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
        {
            // Create a DateTime object representing the token expiration time by adding the number of minutes specified
            // in the configuration to current the UTC 
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:Expiration_Minutes"]));

            // Create an array of Claim objects representing the user's claims, such as their id,email,username etc.
            Claim[] claims = new Claim[]
            {
                //Subject (user id)
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                //Jwt unique id
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //Issued at (date & time of token generation)
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                //Unique name identifier of the user (email)
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                //Name of the user
                new Claim(ClaimTypes.Name, user.PersonName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Create a SymmetricSecurityKey object using the key specified in the configuration
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            // Create a SigningCredentials object with the security key and the HmacSha256 algorithm
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create a JwtSecurityToken object with the given issuer, audience, claims, expires and signingCredentials
            JwtSecurityToken tokengenerator = new JwtSecurityToken(

                _configuration["JWT:Issuer"],
                _configuration["JWT:Audience"],
                claims,
                expires: expiration,
                signingCredentials: signingCredentials
                );

            // Create a JwtSecurityTokenHandler object and use it to write the token as a string 
            JwtSecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler();
            string token = securityTokenHandler.WriteToken(tokengenerator);

            // Create AuthenticationResponse object containing token, personName, email and token expiration time
            var authenticationResponse = new AuthenticationResponse()
            {
                Token = token,
                PersonName = user.PersonName,
                Email = user.Email,
                Expiration = expiration,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDateTime = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["RefreshToken:Expiration_Minutes"])),
            };

            return authenticationResponse;
        }

        public ClaimsPrincipal? GetClaimsPrincipalFromJwtToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:Audience"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:Issuer"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
                ValidateLifetime = false
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal claimsPrincipal = jwtSecurityTokenHandler.ValidateToken(token, 
                                                                tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return claimsPrincipal;
        }

        // Creates a refresh token (base64 string of random numbers)
        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

        #endregion
    }
}
