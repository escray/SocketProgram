using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class Chat : Form
    {
        /// <summary>
        /// 接收数据缓冲区大小 
        /// </summary>
        private int maxPacket = 1024;

        public Chat()
        {
            InitializeComponent();
        }

        public NetworkStream Stream { get; set; }
        public string UserName { get; set; }
        public ChatType ChatType { get; set; }
        public string ReceiveName { get; set; }

        private Thread receiveThread;
       
        public void DisplayMessage(string msg)
        {
            rtbMessageDisplay.AppendText(msg);
            rtbMessageDisplay.ScrollToCaret();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = tbxSendMessage.Text.Trim();
            string localmsg = string.Empty;
            string sendmsg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                MessageBox.Show("不能发送空消息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // BroadCast
            if (ChatType == ChatType.Public)
            {
                localmsg = "[广播]您在 " + DateTime.Now + " 对所有人说\r\n" + msg + "\r\n\r\n";
                sendmsg = "[广播]" + UserName + " 在 " + DateTime.Now + " 对所有人说：\r\n" + msg + "\r\n\r\n";
                byte[] cmdmsg = Encoding.Unicode.GetBytes("cmd::BroadCast");
                Stream.Write(cmdmsg, 0, cmdmsg.Length);
                
            }
            else if(ChatType == ChatType.Private)
            {
                sendmsg = string.Format("[{0}] : {1} {2}\r\n", UserName, msg, DateTime.Now.ToShortTimeString());
                localmsg = sendmsg;

                byte[] rcvName = Encoding.Unicode.GetBytes(ReceiveName);
                Stream.Write(rcvName, 0, rcvName.Length);
            }

            byte[] sendbytes = Encoding.Unicode.GetBytes(sendmsg);
            Stream.Write(sendbytes, 0, sendbytes.Length);
            

            rtbMessageDisplay.AppendText(localmsg);
            tbxSendMessage.Clear();
        }

        private void Chat_Load(object sender, EventArgs e)
        {
        }
    }
}
