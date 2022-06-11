//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Entities
{
    using Entities;
    using System;
    using System.Collections.Generic;
    using TS_Chat;

    public partial class Mensagens
    {
        public int IdMensagem { get; set; }
        public System.DateTime dtCreation { get; set; }
        public byte[] Text { get; set; }
        public int IdUser { get; set; }
        public byte[] key { get; set; }
        public byte[] iv { get; set; }    
        public virtual Users Users { get; set; }
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

        public string GetMessage()
        {
            //Initialize Decryptor
            Cryptor cryptor = new Cryptor();
            //Get key string 
            string key = Convert.ToBase64String(this.key);
            //Get IV string
            string iv = Convert.ToBase64String(this.key);
            //Get Message Text string
            string text = Convert.ToBase64String(this.key);
            //Decript message
            return cryptor.DesencryptText(key, iv, text);
        }
    }
}
