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
            this.chats_list = new System.Windows.Forms.ListBox();
            this.tb_chat = new System.Windows.Forms.TextBox();
            this.tb_message = new System.Windows.Forms.TextBox();
            this.bt_send = new System.Windows.Forms.Button();
            this.lb_chat = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chats_list
            // 
            this.chats_list.FormattingEnabled = true;
            this.chats_list.ItemHeight = 15;
            this.chats_list.Location = new System.Drawing.Point(12, 12);
            this.chats_list.Name = "chats_list";
            this.chats_list.Size = new System.Drawing.Size(146, 424);
            this.chats_list.TabIndex = 0;
            // 
            // tb_chat
            // 
            this.tb_chat.Location = new System.Drawing.Point(164, 52);
            this.tb_chat.Multiline = true;
            this.tb_chat.Name = "tb_chat";
            this.tb_chat.Size = new System.Drawing.Size(624, 355);
            this.tb_chat.TabIndex = 1;
            // 
            // tb_message
            // 
            this.tb_message.Location = new System.Drawing.Point(164, 413);
            this.tb_message.Name = "tb_message";
            this.tb_message.PlaceholderText = "Escrever...";
            this.tb_message.Size = new System.Drawing.Size(543, 23);
            this.tb_message.TabIndex = 2;
            // 
            // bt_send
            // 
            this.bt_send.Location = new System.Drawing.Point(713, 413);
            this.bt_send.Name = "bt_send";
            this.bt_send.Size = new System.Drawing.Size(75, 23);
            this.bt_send.TabIndex = 3;
            this.bt_send.Text = "Enviar";
            this.bt_send.UseVisualStyleBackColor = true;
            this.bt_send.Click += new System.EventHandler(this.bt_send_Click);
            // 
            // lb_chat
            // 
            this.lb_chat.AutoSize = true;
            this.lb_chat.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lb_chat.Location = new System.Drawing.Point(164, 12);
            this.lb_chat.Name = "lb_chat";
            this.lb_chat.Size = new System.Drawing.Size(93, 37);
            this.lb_chat.TabIndex = 4;
            this.lb_chat.Text = "Name";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lb_chat);
            this.Controls.Add(this.bt_send);
            this.Controls.Add(this.tb_message);
            this.Controls.Add(this.tb_chat);
            this.Controls.Add(this.chats_list);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox chats_list;
        private TextBox tb_chat;
        private TextBox tb_message;
        private Button bt_send;
        private Label lb_chat;
    }
}