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
    public partial class Chat : Form
    {
        public Chat()
        {
            InitializeComponent();
        }

        public NetworkStream Stream { get; set; }
        public string UserName { get; set; }

        

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = tbxSendMessage.Text.Trim();
            string localmsg;
            string sendmsg;
            if (string.IsNullOrEmpty(msg))
            {
                MessageBox.Show("不能发送空消息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            localmsg = "[广播]您在 " + DateTime.Now + " 对所有人说\r\n" + msg + "\r\n\r\n";
            sendmsg = "[广播]" + UserName + " 在 " + DateTime.Now + " 对所有人说：\r\n" + msg + "\r\n\r\n";
            byte[] cmdmsg = Encoding.Unicode.GetBytes("cmd::BroadCast");
            Stream.Write(cmdmsg, 0, cmdmsg.Length);
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
