using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GrpcJwt.Utils
{
    public class MD5Helper
    {
       public static string GetMD5(string value)
        {
            var input = Encoding.UTF8.GetBytes(value);    //tbPass为输入密码的文本框
            var cryptoProvider = new MD5CryptoServiceProvider();
            byte[] output = cryptoProvider.ComputeHash(input);
            return BitConverter.ToString(output).Replace("-", "");
        }
    }
}
