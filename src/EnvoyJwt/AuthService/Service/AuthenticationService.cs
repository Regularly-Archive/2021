using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Service
{
    public class AuthenticateService : IAuthenticationService
    {
        private JwtConfig _jwtConfig;
        private IUserInfoService _userInfoService;

        public AuthenticateService(IUserInfoService userInfoService, JwtConfig jwtConfig)
        {
            _userInfoService = userInfoService;
            _jwtConfig = jwtConfig;
        }

        public bool IsAuthenticated(PasswordRequest request, out string token)
        {
            token = string.Empty;

            if (!_userInfoService.IsValid(request))
                return false;

            var claims = new[] { new Claim(ClaimTypes.Name, request.Username) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                _jwtConfig.Issuer,
                _jwtConfig.Audience, 
                claims, 
                expires: DateTime.Now.AddMinutes(_jwtConfig.AccessTokenExpire), 
                signingCredentials: credentials
            );

            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return true;
        }
    }
}
