using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcJwt.Services
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(LoginRequest request, out string token);
    }
}
