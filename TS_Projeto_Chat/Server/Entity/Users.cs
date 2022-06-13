using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public partial class Users
    {
        public Users(string username, byte[] salt, byte[] saltedPasswordHash)
        {
            this.Username = username;
            this.SaltedPasswordHash = saltedPasswordHash;
            this.Salt = salt;
            this.dtCreation = DateTime.Now;
            this.Mensagens = new HashSet<Mensagens>();
        }
        public bool checkedSaltPassword(byte[] SaltedPassword)
        {
            return this.SaltedPasswordHash.SequenceEqual(SaltedPassword);
        }
    }
}
