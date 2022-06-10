
﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TS_Chat
{
    internal class Cryptor
    {
        private int SALT_SIZE = 8;
        private int ITERATIONS = 1000;
        private AesCryptoServiceProvider AES;

        public Cryptor()
        {
            this.AES = new AesCryptoServiceProvider();
        }

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

        //Create Simetric Key from string
        public string CreatePrivateKey(string pass)
        {
            byte[] salt = new byte[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };
            Rfc2898DeriveBytes pwgen = new Rfc2898DeriveBytes(pass, salt, this.ITERATIONS);
            //Generate key
            byte[] key = pwgen.GetBytes(16);
            //Converte key to Base64 and return
            return Convert.ToBase64String(key);
        }

        //Create Initial Vector from string
        public string CreateIV(string pass)
        {
            byte[] salt = new byte[] { 3, 14, 15, 92, 65, 35, 89, 79, 32, 38, 46 };
            Rfc2898DeriveBytes pwgen = new Rfc2898DeriveBytes(pass, salt, ITERATIONS);
            //Generate initicial vetor
            byte[] iv = pwgen.GetBytes(16);
            //Converte initicial vetor to Base64 and return
            return Convert.ToBase64String(iv);
        }

        //Encrypt a string with AES 
        public string EncryptText(string text_to_encrypt)
        {
            //Save desencrypted text as Bytes
            byte[] desencrypted_text = Encoding.UTF8.GetBytes(text_to_encrypt);
            //Save encryoted text as Bytes
            byte[] encrypted_text;
            //Save memory space
            MemoryStream ms = new MemoryStream();
            //Initialize encrypted sistem
            CryptoStream cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write);
            //Encrypt data
            cs.Write(desencrypted_text, 0, desencrypted_text.Length);
            cs.Close();
            //Save encrypted data from memory
            encrypted_text = ms.ToArray();
            //Convert from Byte -> Base64 and return
            return Convert.ToBase64String(encrypted_text);
        }

        //Desencrypt string
        public string DesencryptText(string text_encrypted)
        {
            //Save desencrypted text as Bytes
            byte[] encrypted_text = Convert.FromBase64String(text_encrypted);
            //Save memory space
            MemoryStream ms = new MemoryStream(encrypted_text);
            //Initialize encrypted sistem
            CryptoStream cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Read);
            //Save desencrypt text
            byte[] desencrypted_text = new byte[ms.Length];
            //Number of bytes desencrypted
            int readBytes = cs.Read(desencrypted_text, 0, desencrypted_text.Length);
            cs.Close();
            //Convert from Byte -> Base64 and return
            return Encoding.UTF8.GetString(desencrypted_text, 0, readBytes);
        }

    }
}
