using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Service
{
    public interface IUserInfoService
    {
        bool IsValid(PasswordRequest request);
    }
}
