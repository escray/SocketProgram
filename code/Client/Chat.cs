using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class Chat : Form
    {

        public Chat()
        {
            InitializeComponent();
        }

        public NetworkStream Stream { get; set; }
        public string UserName { get; set; }
        public ChatType ChatType { get; set; }
        public string ReceiveName { get; set; }

        public void DisplayMessage(string msg)
        {
            rtbMessageDisplay.AppendText(msg);
            rtbMessageDisplay.ScrollToCaret();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            var main = Parent as Main;
            if (main != null)
            {
                main.ChatButton.Enabled = true;
                main.BroadCastButton.Enabled = true;
            }

            Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = tbxSendMessage.Text.Trim();
            string localmsg = string.Empty;
            string sendmsg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(@"不能发送空消息", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // BroadCast
            if (ChatType == ChatType.Public)
            {
                localmsg = "[广播]您在 " + DateTime.Now + " 对所有人说\r\n" + msg + "\r\n\r\n";
                sendmsg = "[广播]" + UserName + " 在 " + DateTime.Now + " 对所有人说：\r\n" + msg + "\r\n\r\n";

                
                SendMessage(Stream, "cmd::BroadCast");
            }
            else if(ChatType == ChatType.Private)
            {
                sendmsg = string.Format("[{0}] : {1} {2}\r\n", UserName, msg, DateTime.Now.ToShortTimeString());
                localmsg = sendmsg;

                SendMessage(Stream, ReceiveName);
            }

            SendMessage(Stream, sendmsg);

            DisplayMessage(localmsg);
            tbxSendMessage.Clear();
        }

        private void SendMessage(NetworkStream stream, string msg)
        {
            try
            {
                byte[] cmdmsg = Encoding.Unicode.GetBytes(msg);
                stream.Write(cmdmsg, 0, cmdmsg.Length);
            }
            catch (Exception)
            {
                MessageBox.Show(@"网络中断，无法连接到服务器，请重试！", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
        }

        private void Chat_Load(object sender, EventArgs e)
        {
        }
    }
}
