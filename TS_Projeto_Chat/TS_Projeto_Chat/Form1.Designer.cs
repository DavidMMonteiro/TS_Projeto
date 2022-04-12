﻿namespace TS_Projeto_Chat
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
            this.chats_list = new System.Windows.Forms.ListBox();
            this.tb_chat = new System.Windows.Forms.TextBox();
            this.tb_message = new System.Windows.Forms.TextBox();
            this.bt_send = new System.Windows.Forms.Button();
            this.lb_chat = new System.Windows.Forms.Label();
            this.bt_connect = new System.Windows.Forms.Button();
            this.bt_logout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chats_list
            // 
            this.chats_list.FormattingEnabled = true;
            this.chats_list.ItemHeight = 20;
            this.chats_list.Location = new System.Drawing.Point(14, 16);
            this.chats_list.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chats_list.Name = "chats_list";
            this.chats_list.Size = new System.Drawing.Size(166, 564);
            this.chats_list.TabIndex = 0;
            // 
            // tb_chat
            // 
            this.tb_chat.Enabled = false;
            this.tb_chat.Location = new System.Drawing.Point(187, 69);
            this.tb_chat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tb_chat.Multiline = true;
            this.tb_chat.Name = "tb_chat";
            this.tb_chat.Size = new System.Drawing.Size(713, 472);
            this.tb_chat.TabIndex = 1;
            // 
            // tb_message
            // 
            this.tb_message.Location = new System.Drawing.Point(187, 551);
            this.tb_message.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tb_message.Name = "tb_message";
            this.tb_message.PlaceholderText = "Escrever...";
            this.tb_message.Size = new System.Drawing.Size(620, 27);
            this.tb_message.TabIndex = 2;
            this.tb_message.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_message_KeyPress);
            // 
            // bt_send
            // 
            this.bt_send.Location = new System.Drawing.Point(815, 551);
            this.bt_send.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bt_send.Name = "bt_send";
            this.bt_send.Size = new System.Drawing.Size(86, 31);
            this.bt_send.TabIndex = 3;
            this.bt_send.Text = "Enviar";
            this.bt_send.UseVisualStyleBackColor = true;
            this.bt_send.Click += new System.EventHandler(this.bt_send_Click);
            // 
            // lb_chat
            // 
            this.lb_chat.AutoSize = true;
            this.lb_chat.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lb_chat.Location = new System.Drawing.Point(187, 16);
            this.lb_chat.Name = "lb_chat";
            this.lb_chat.Size = new System.Drawing.Size(190, 46);
            this.lb_chat.TabIndex = 4;
            this.lb_chat.Text = "Test_Client";
            // 
            // bt_connect
            // 
            this.bt_connect.Enabled = false;
            this.bt_connect.Location = new System.Drawing.Point(761, 16);
            this.bt_connect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bt_connect.Name = "bt_connect";
            this.bt_connect.Size = new System.Drawing.Size(139, 31);
            this.bt_connect.TabIndex = 5;
            this.bt_connect.Text = "Connect to Chat";
            this.bt_connect.UseVisualStyleBackColor = true;
            this.bt_connect.Click += new System.EventHandler(this.bt_connect_Click);
            // 
            // bt_logout
            // 
            this.bt_logout.Location = new System.Drawing.Point(616, 16);
            this.bt_logout.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bt_logout.Name = "bt_logout";
            this.bt_logout.Size = new System.Drawing.Size(139, 31);
            this.bt_logout.TabIndex = 5;
            this.bt_logout.Text = "Logout";
            this.bt_logout.UseVisualStyleBackColor = true;
            this.bt_logout.Click += new System.EventHandler(this.bt_logout_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 600);
            this.Controls.Add(this.bt_logout);
            this.Controls.Add(this.bt_connect);
            this.Controls.Add(this.lb_chat);
            this.Controls.Add(this.bt_send);
            this.Controls.Add(this.tb_message);
            this.Controls.Add(this.tb_chat);
            this.Controls.Add(this.chats_list);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Chats";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox chats_list;
        private TextBox tb_chat;
        private TextBox tb_message;
        private Button bt_send;
        private Label lb_chat;
        private Button bt_connect;
        private Button bt_logout;
    }
}