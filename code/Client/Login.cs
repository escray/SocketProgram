using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private IPAddress ipAddr;
        private int port = 11000;

        

        public string UserName
        {
            get { return tbxUserName.Text.Trim();  }
            set { tbxUserName.Text = value.Trim(); }
        }

        public string ServerIPAddress
        {
            get { return tbxServer.Text.Trim(); }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            tbxServer.Text = "127.0.0.1";
            tbxUserName.Focus();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool ValidateLoginInfo()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                MessageBox.Show("请填写用户名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (!IPAddress.TryParse(tbxServer.Text.Trim(), out ipAddr))
            {
                MessageBox.Show("IP 地址不合法", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!ValidateLoginInfo())
            {
                return;
            }


            var tcpClient = new TcpClient();
            tcpClient.Connect(ipAddr, port);

            if (!tcpClient.Connected)
            {
                MessageBox.Show("", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            NetworkStream stream = tcpClient.GetStream();
            stream.Write(Encoding.Unicode.GetBytes(UserName), 0, Encoding.Unicode.GetBytes(UserName).Length);

            byte[] buffer = new byte[512];
            stream.Read(buffer, 0, buffer.Length);
            string connResult = Encoding.Unicode.GetString(buffer).TrimEnd('\0');
            if (connResult.Equals("cmd::Failed"))
            {
                MessageBox.Show("用户名已被使用", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            string serverSocket = ServerIPAddress + ":" + port;

            Main mainForm = new Main(UserName, serverSocket, stream);

            mainForm.Owner = this;
            Hide();
            mainForm.Show();
        }
    }
}