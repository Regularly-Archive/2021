using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Service
{
    public class UserInfoService : IUserInfoService
    {

        public bool IsValid(PasswordRequest request)
        {
            return true;
        }
    }
}
