using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcJwt
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string Password { get; set; }
    }
}
