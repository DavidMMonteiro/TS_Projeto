using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TS_Chat
{
    public partial class FormSignUp : Form
    {
        private Form FormBack;
        public FormSignUp(Form formBack)
        {
            InitializeComponent();
            this.FormBack = formBack;
            this.FormBack.Hide();
        }

        private void bt_cancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Quere sair do SigUp?", "Exit Signup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                this.FormBack.Show();
                this.Close();
            }

        }

        private void bt_registar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_username.Text))
            {
                MessageBox.Show("Insira os dados no cambo username", "Sign Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if(string.IsNullOrEmpty(tb_password.Text) || string.IsNullOrEmpty(tb_re_password.Text))
            {
                MessageBox.Show("Insira os dados nos campos de password", "Sign Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (tb_password.Text == tb_re_password.Text)
            {
                MessageBox.Show("As password tem de ser iguais!","Sign Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tb_re_password.Text = "";
                return;
            }

            //TODO Encryptar password
            //TODO Encryptar mensagem para o servidor

        }
    }
}
