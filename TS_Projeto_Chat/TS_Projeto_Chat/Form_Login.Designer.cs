﻿namespace TS_Projeto_Chat
{
    partial class Form_Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tb_user = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_password = new System.Windows.Forms.MaskedTextBox();
            this.bt_login = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(106, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username";
            // 
            // tb_user
            // 
            this.tb_user.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_user.Location = new System.Drawing.Point(214, 151);
            this.tb_user.Name = "tb_user";
            this.tb_user.PlaceholderText = "Admin";
            this.tb_user.Size = new System.Drawing.Size(157, 27);
            this.tb_user.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(106, 189);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 28);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password:";
            // 
            // tb_password
            // 
            this.tb_password.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_password.Location = new System.Drawing.Point(214, 190);
            this.tb_password.Name = "tb_password";
            this.tb_password.Size = new System.Drawing.Size(157, 27);
            this.tb_password.TabIndex = 3;
            // 
            // bt_login
            // 
            this.bt_login.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bt_login.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.bt_login.Location = new System.Drawing.Point(106, 231);
            this.bt_login.Name = "bt_login";
            this.bt_login.Size = new System.Drawing.Size(265, 29);
            this.bt_login.TabIndex = 4;
            this.bt_login.Text = "Login";
            this.bt_login.UseVisualStyleBackColor = true;
            this.bt_login.Click += new System.EventHandler(this.bt_login_Click);
            // 
            // Form_Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 453);
            this.Controls.Add(this.bt_login);
            this.Controls.Add(this.tb_password);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_user);
            this.Controls.Add(this.label1);
            this.Name = "Form_Login";
            this.Text = "Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Login_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox tb_user;
        private Label label2;
        private MaskedTextBox tb_password;
        private Button bt_login;
    }
}