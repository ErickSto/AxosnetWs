using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Axosnet.Helpers
{
    public class Hashing
    {
        private static string GetRangomSalt() {
            return BCrypt.Net.BCrypt.GenerateSalt(12);
        }

        public static string HashPassword(string password) {
            return BCrypt.Net.BCrypt.HashPassword(password, GetRangomSalt());
        }

        public static bool validatePassword(string password, string correctHash) {
            return BCrypt.Net.BCrypt.Verify(password, correctHash);
        }
    }
}