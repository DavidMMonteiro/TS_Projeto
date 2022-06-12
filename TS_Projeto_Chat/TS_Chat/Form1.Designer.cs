using System.Windows.Forms;

namespace TS_Projeto_Chat
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tb_chat = new System.Windows.Forms.TextBox();
            this.tb_message = new System.Windows.Forms.TextBox();
            this.bt_send = new System.Windows.Forms.Button();
            this.lb_chat = new System.Windows.Forms.Label();
            this.bt_connect = new System.Windows.Forms.Button();
            this.bt_logout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tb_chat
            // 
            this.tb_chat.Location = new System.Drawing.Point(10, 45);
            this.tb_chat.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tb_chat.Multiline = true;
            this.tb_chat.Name = "tb_chat";
            this.tb_chat.ReadOnly = true;
            this.tb_chat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_chat.Size = new System.Drawing.Size(665, 308);
            this.tb_chat.TabIndex = 1;
            // 
            // tb_message
            // 
            this.tb_message.Location = new System.Drawing.Point(11, 358);
            this.tb_message.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tb_message.Name = "tb_message";
            this.tb_message.Size = new System.Drawing.Size(595, 20);
            this.tb_message.TabIndex = 2;
            this.tb_message.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_message_KeyPress);
            // 
            // bt_send
            // 
            this.bt_send.Location = new System.Drawing.Point(611, 358);
            this.bt_send.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.bt_send.Name = "bt_send";
            this.bt_send.Size = new System.Drawing.Size(64, 20);
            this.bt_send.TabIndex = 3;
            this.bt_send.Text = "Enviar";
            this.bt_send.UseVisualStyleBackColor = true;
            this.bt_send.Click += new System.EventHandler(this.bt_send_Click);
            // 
            // lb_chat
            // 
            this.lb_chat.AccessibleRole = System.Windows.Forms.AccessibleRole.IpAddress;
            this.lb_chat.AutoSize = true;
            this.lb_chat.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold);
            this.lb_chat.Location = new System.Drawing.Point(11, 5);
            this.lb_chat.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lb_chat.Name = "lb_chat";
            this.lb_chat.Size = new System.Drawing.Size(155, 37);
            this.lb_chat.TabIndex = 4;
            this.lb_chat.Text = "Test_Client";
            // 
            // bt_connect
            // 
            this.bt_connect.Enabled = false;
            this.bt_connect.Location = new System.Drawing.Point(571, 10);
            this.bt_connect.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.bt_connect.Name = "bt_connect";
            this.bt_connect.Size = new System.Drawing.Size(104, 29);
            this.bt_connect.TabIndex = 5;
            this.bt_connect.Text = "Connect to Chat";
            this.bt_connect.UseVisualStyleBackColor = true;
            this.bt_connect.Click += new System.EventHandler(this.bt_connect_Click);
            // 
            // bt_logout
            // 
            this.bt_logout.Location = new System.Drawing.Point(462, 10);
            this.bt_logout.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.bt_logout.Name = "bt_logout";
            this.bt_logout.Size = new System.Drawing.Size(104, 29);
            this.bt_logout.TabIndex = 5;
            this.bt_logout.Text = "Logout";
            this.bt_logout.UseVisualStyleBackColor = true;
            this.bt_logout.Click += new System.EventHandler(this.bt_logout_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 386);
            this.Controls.Add(this.bt_logout);
            this.Controls.Add(this.bt_connect);
            this.Controls.Add(this.lb_chat);
            this.Controls.Add(this.bt_send);
            this.Controls.Add(this.tb_message);
            this.Controls.Add(this.tb_chat);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "Form1";
            this.Text = "Chats";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TextBox tb_chat;
        private TextBox tb_message;
        private Button bt_send;
        private Label lb_chat;
        private Button bt_connect;
        private Button bt_logout;
    }
}