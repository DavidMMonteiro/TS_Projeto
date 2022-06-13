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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tb_chat = new System.Windows.Forms.TextBox();
            this.tb_message = new System.Windows.Forms.TextBox();
            this.bt_send = new System.Windows.Forms.Button();
            this.lb_chat = new System.Windows.Forms.Label();
            this.bt_connect = new System.Windows.Forms.Button();
            this.bt_logout = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_chat
            // 
            this.tb_chat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_chat.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_chat.Location = new System.Drawing.Point(0, 41);
            this.tb_chat.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tb_chat.MaxLength = 100000;
            this.tb_chat.Multiline = true;
            this.tb_chat.Name = "tb_chat";
            this.tb_chat.ReadOnly = true;
            this.tb_chat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_chat.Size = new System.Drawing.Size(785, 406);
            this.tb_chat.TabIndex = 5;
            // 
            // tb_message
            // 
            this.tb_message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_message.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_message.Location = new System.Drawing.Point(0, 0);
            this.tb_message.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tb_message.MaxLength = 300;
            this.tb_message.Multiline = true;
            this.tb_message.Name = "tb_message";
            this.tb_message.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_message.Size = new System.Drawing.Size(785, 31);
            this.tb_message.TabIndex = 1;
            this.tb_message.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_message_KeyPress);
            // 
            // bt_send
            // 
            this.bt_send.Dock = System.Windows.Forms.DockStyle.Right;
            this.bt_send.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_send.Location = new System.Drawing.Point(721, 0);
            this.bt_send.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.bt_send.Name = "bt_send";
            this.bt_send.Size = new System.Drawing.Size(64, 31);
            this.bt_send.TabIndex = 2;
            this.bt_send.Text = "Enviar";
            this.bt_send.UseVisualStyleBackColor = true;
            this.bt_send.Click += new System.EventHandler(this.bt_send_Click);
            // 
            // lb_chat
            // 
            this.lb_chat.AccessibleRole = System.Windows.Forms.AccessibleRole.IpAddress;
            this.lb_chat.AutoSize = true;
            this.lb_chat.Dock = System.Windows.Forms.DockStyle.Left;
            this.lb_chat.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold);
            this.lb_chat.Location = new System.Drawing.Point(0, 0);
            this.lb_chat.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lb_chat.Name = "lb_chat";
            this.lb_chat.Size = new System.Drawing.Size(155, 37);
            this.lb_chat.TabIndex = 4;
            this.lb_chat.Text = "Test_Client";
            // 
            // bt_connect
            // 
            this.bt_connect.Dock = System.Windows.Forms.DockStyle.Right;
            this.bt_connect.Enabled = false;
            this.bt_connect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_connect.Location = new System.Drawing.Point(681, 0);
            this.bt_connect.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.bt_connect.Name = "bt_connect";
            this.bt_connect.Size = new System.Drawing.Size(104, 41);
            this.bt_connect.TabIndex = 4;
            this.bt_connect.Text = "Connect to Chat";
            this.bt_connect.UseVisualStyleBackColor = true;
            this.bt_connect.Click += new System.EventHandler(this.bt_connect_Click);
            // 
            // bt_logout
            // 
            this.bt_logout.Dock = System.Windows.Forms.DockStyle.Right;
            this.bt_logout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_logout.Location = new System.Drawing.Point(577, 0);
            this.bt_logout.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.bt_logout.Name = "bt_logout";
            this.bt_logout.Size = new System.Drawing.Size(104, 41);
            this.bt_logout.TabIndex = 3;
            this.bt_logout.Text = "Logout";
            this.bt_logout.UseVisualStyleBackColor = true;
            this.bt_logout.Click += new System.EventHandler(this.bt_logout_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lb_chat);
            this.panel1.Controls.Add(this.bt_logout);
            this.panel1.Controls.Add(this.bt_connect);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(785, 41);
            this.panel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tb_chat);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(785, 478);
            this.panel2.TabIndex = 7;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.bt_send);
            this.panel3.Controls.Add(this.tb_message);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 447);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(785, 31);
            this.panel3.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 478);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "Form1";
            this.Text = "Chats";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private TextBox tb_chat;
        private TextBox tb_message;
        private Button bt_send;
        private Label lb_chat;
        private Button bt_connect;
        private Button bt_logout;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
    }
}