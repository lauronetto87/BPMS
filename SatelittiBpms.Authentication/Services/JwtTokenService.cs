using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Satelitti.Authentication.Authorization;
using Satelitti.Authentication.Service.Interface;
using Satelitti.Authentication.Types;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Options.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SatelittiBpms.Authentication.Services
{
    public class JwtTokenService : ITokenService<GenerateTokenParameters>
    {
        private readonly AuthenticationOptions _authenticationOptions;

        public JwtTokenService(
            IOptions<AuthenticationOptions> authenticationOptions)
        {
            _authenticationOptions = authenticationOptions.Value;
        }

        public AuthToken GenerateToken(GenerateTokenParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (_authenticationOptions.SecretKey == null)
            {
                throw new ArgumentNullException(nameof(_authenticationOptions.SecretKey));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authenticationOptions.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(GenerateClaimProps(parameters)),
                Expires = DateTime.UtcNow.AddMinutes(parameters.TokenLifetimeInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var authToken = new AuthToken()
            {
                Token = tokenHandler.WriteToken(token),
                ExpiresIn = (int)TimeSpan.FromMinutes(parameters.TokenLifetimeInMinutes).TotalSeconds
            };

            return authToken;
        }

        private static IEnumerable<Claim> GenerateClaimProps(GenerateTokenParameters parameters)
        {
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Role, parameters.Role),
                new Claim(ClaimTypes.Name, parameters.Name),
                new Claim(ClaimTypes.Sid, parameters.Sid),
                new Claim(ClaimTypes.Email, parameters.Email),
                new Claim(CustomClaimTypes.SUITE_TOKEN, parameters.GetSuiteToken()),
                new Claim(CustomClaimTypes.TIMEZONE, parameters.GetTimezone())
            };

            return claims;
        }
    }
}
