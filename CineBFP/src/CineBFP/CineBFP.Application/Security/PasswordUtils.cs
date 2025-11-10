using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Application.Security
{
    public static class PasswordUtils
    {
        public static string HashBcrypt(string plain, int workFactor = 12)
            => BCrypt.Net.BCrypt.HashPassword(plain, workFactor);

        public static bool Verify(string plain, string hash)
            => BCrypt.Net.BCrypt.Verify(plain, hash);
    }
}
