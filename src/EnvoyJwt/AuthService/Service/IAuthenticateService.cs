using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Service
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated(PasswordRequest request, out string token);
    }
}
