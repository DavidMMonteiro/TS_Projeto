using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    partial class Mensagens
    {
        public Mensagens(string text, Users user) : base()
        {
            this.Text = text;
            this.Users = user;
            this.dtCreation = DateTime.Now;
        }

        internal void SetUser()
        {
            string username = this.Users.Username;
            this.Users = new Users();
            this.Users.Username = username;
        }
    }
}
