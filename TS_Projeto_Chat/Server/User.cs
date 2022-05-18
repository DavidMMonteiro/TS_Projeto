using System;

namespace TS_Chat
{
    // Class User para guardar a informação do utilizador
    public class User
    {

        public String Username { get; set; }
        private String Password;

        //User constructor
        public User(String user, String password)
        {
            this.Username = user;
            this.Password = password;
        }

        //Valida a password do utilizador
        public bool checkPassword(string tmpPassword)
        {
            return (tmpPassword != null && tmpPassword == this.Password);
        }
    }
}
