namespace Client
{
    partial class Main
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbxOnline = new System.Windows.Forms.ListBox();
            this.btnChatRoom = new System.Windows.Forms.Button();
            this.btnChat = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbxOnline);
            this.groupBox1.Location = new System.Drawing.Point(3, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(196, 411);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "当前在线";
            // 
            // lbxOnline
            // 
            this.lbxOnline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxOnline.FormattingEnabled = true;
            this.lbxOnline.ItemHeight = 12;
            this.lbxOnline.Location = new System.Drawing.Point(3, 17);
            this.lbxOnline.Name = "lbxOnline";
            this.lbxOnline.Size = new System.Drawing.Size(190, 391);
            this.lbxOnline.TabIndex = 0;
            // 
            // btnChatRoom
            // 
            this.btnChatRoom.Location = new System.Drawing.Point(11, 443);
            this.btnChatRoom.Name = "btnChatRoom";
            this.btnChatRoom.Size = new System.Drawing.Size(51, 23);
            this.btnChatRoom.TabIndex = 1;
            this.btnChatRoom.Text = "聊天室";
            this.btnChatRoom.UseVisualStyleBackColor = true;
            this.btnChatRoom.Click += new System.EventHandler(this.btnChatRoom_Click);
            // 
            // btnChat
            // 
            this.btnChat.Location = new System.Drawing.Point(74, 443);
            this.btnChat.Name = "btnChat";
            this.btnChat.Size = new System.Drawing.Size(51, 23);
            this.btnChat.TabIndex = 2;
            this.btnChat.Text = "聊天";
            this.btnChat.UseVisualStyleBackColor = true;
            this.btnChat.Click += new System.EventHandler(this.btnChat_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Location = new System.Drawing.Point(137, 443);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(51, 23);
            this.btnQuit.TabIndex = 3;
            this.btnQuit.Text = "退出";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(201, 480);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnChat);
            this.Controls.Add(this.btnChatRoom);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "主界面";
            this.Load += new System.EventHandler(this.Main_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnChatRoom;
        private System.Windows.Forms.Button btnChat;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.ListBox lbxOnline;
    }
}