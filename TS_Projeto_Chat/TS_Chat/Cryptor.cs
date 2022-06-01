using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TS_Chat
{
    internal class Cryptor
    {
        private int SALT_SIZE = 25;
        private int ITERATIONS = 10000;
        public byte[] GenerateSalt()
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[this.SALT_SIZE];
            rng.GetBytes(buff);
            return buff;
        }
        public byte[] GenerateSaltedHash(string plainText, byte[] salt)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(plainText, salt, this.ITERATIONS);
            return rfc2898.GetBytes(32);
        }

    }
}
