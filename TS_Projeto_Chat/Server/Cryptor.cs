using System.Security.Cryptography;

namespace TS_Chat
{
    internal class Cryptor
    {
        private int SALT_SIZE = 8;
        private int ITERATIONS = 1000;

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
