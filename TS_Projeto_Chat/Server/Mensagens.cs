//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server
{
    using System;
    using System.Collections.Generic;
    
    public partial class Mensagens
    {
        public Mensagens()
        {
            this.Users = new Users();
            this.dtCreation = DateTime.Now;
        }

        public Mensagens(string text, Users user) : base()
        {           
            this.Text = text;
            this.Users = user;
            this.dtCreation = DateTime.Now;
        }

        public int IdMensagem { get; set; }
        public System.DateTime dtCreation { get; set; }
        public string Text { get; set; }    
        public virtual Users Users { get; set; }
    }
}
