using System;
using System.Collections.Generic;
using System.Linq;
using TS_Chat;

namespace Server
{
    partial class Mensagens
    {
        public Mensagens()
        {
            this.dtCreation = DateTime.Now;
        }
        public Mensagens(string text, Users user) : base()
        {
            this.Text = Convert.FromBase64String(text);
            this.Users = user;
            this.dtCreation = DateTime.Now;
        }
        internal void SetUser()
        {
            string username = this.Users.Username;
            this.Users = new Users();
            this.Users.Username = username;
        }
        internal void SetUser(Users users)
        {
            this.Users = new Users();
            this.Users.IdUser = users.IdUser;
            this.Users.Username = users.Username;
        }

    }
}
