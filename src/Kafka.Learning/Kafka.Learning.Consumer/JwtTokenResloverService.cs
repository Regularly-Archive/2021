using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Kafka.Learning.Consumer
{
    public class JwtTokenResloverService
    {
        private readonly JwtSecurityTokenHandler _jwtHandler = new JwtSecurityTokenHandler();
        private readonly JwtOptions _jwtOptions;

        public JwtTokenResloverService()
        {
            _jwtOptions = new JwtOptions()
            {
                Secret = "abcdefghijklmnopqrstuvwxyz",
                Issuer = "GrpcJwt",
                Audience = "GrpcJwt",
                AccessExpiration = 1440,
                RefreshExpiration = 1440
            };
        }

        public UserInfo ValidateToken(string token)
        {
            var tokenParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret)),
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                ValidateIssuer = true,
                ValidateAudience = true,
            };

            var claimsPrincipal = _jwtHandler.ValidateToken(token, tokenParameters, out SecurityToken securityToken);
            if (claimsPrincipal != null)
            {
                var userInfo = new UserInfo();
                userInfo.UserName = claimsPrincipal.Identity.Name;
                userInfo.UserRole = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                return userInfo;
            }

            return null;
        }
    }
}
