using GrpcJwt.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GrpcJwt.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private IList<UserInfo> _userInfos = new List<UserInfo>()
        {
            new UserInfo(){UserName = "管理员", UserRole = "Admin", Password = MD5Helper.GetMD5("123456") },
            new UserInfo(){UserName = "张三", UserRole = "User", Password = MD5Helper.GetMD5("zhang3") },
        };

        private readonly IOptions<JwtOptions> _jwtOptions;
        public AuthenticateService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions;
        }

        public bool IsAuthenticated(LoginRequest request, out string token)
        {
            token = string.Empty;

            var userInfo = _userInfos.FirstOrDefault(x => x.UserName == request.UserName && x.Password == MD5Helper.GetMD5(request.Password));
            if (userInfo == null) 
                return false;

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userInfo.UserName),
                new Claim(ClaimTypes.Role, userInfo.UserRole)
            };

            //var crtFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Jwt.pfx");
            ///var signKey = new X509SecurityKey(new X509Certificate2(crtFile,"12345678"));
            var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Secret));
            var credentials = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                _jwtOptions.Value.Issuer,
                _jwtOptions.Value.Audience, 
                claims, 
                expires: DateTime.Now.AddMinutes(_jwtOptions.Value.AccessExpiration), 
                signingCredentials: credentials
            );

            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return true;
        }
    }
}
