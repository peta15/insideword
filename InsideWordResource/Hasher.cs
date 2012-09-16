using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace InsideWordResource
{
    static public class Hasher
    {
        static private SHA256Managed _hasher;

        // Static constructor
        static Hasher()
        {
            _hasher = new SHA256Managed();
        }

        static public string HashPassword(string password)
        {
            string paddedPwd = "";
            if (!string.IsNullOrEmpty(password))
            {
                const int minSize = 17;
                int multipule = (minSize / password.Length);
                int remainder = 0;
                if (multipule <= 0)
                {
                    multipule = 1;
                }
                else
                {
                    remainder = minSize % password.Length;
                }

                for (int count = 0; count < multipule; count++)
                {
                    paddedPwd += password;
                }
                paddedPwd += password.Substring(0, remainder);
            }

            byte[] byteString = Encoding.Unicode.GetBytes(paddedPwd);
            byte[] hashedString = _hasher.ComputeHash(byteString);
            return Convert.ToBase64String(hashedString);
        }
    }
}
