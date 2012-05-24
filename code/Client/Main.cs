using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Main : Form
    {
        public NetworkStream Stream { get; set; }
        public string UserName { get; set; }
        public string ServerSocket { get; set; }

        public Main()
        {
            InitializeComponent();
        }

        public Main(string userName, string serverSocket, NetworkStream stream) : this()
        {
            UserName = userName;
            ServerSocket = serverSocket;
            Stream = stream;
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        // Chat Room
        private void btnChatRoom_Click(object sender, EventArgs e)
        {
            Chat chat = new Chat();
            chat.Stream = Stream;
            chat.UserName = UserName;
            chat.Text = "聊天室";
            chat.Show();
        }

        // Talk
        private void btnChat_Click(object sender, EventArgs e)
        {
            Chat chat = new Chat();
            chat.Stream = Stream;
            chat.UserName = UserName;
            chat.Text = "私聊";
            chat.Show();
        }


    }
}
