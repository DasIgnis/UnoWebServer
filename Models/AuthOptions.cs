using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoServer.Models
{
    public class AuthOptions
    {
        public string Issuer { get; set; } // издатель токена
        public string Audience { get; set; } // потребитель токена
        public string Key { get; set; }  // ключ для шифрации
        public int Lifetime { get; set; } // время жизни токена - 1 минута
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}
