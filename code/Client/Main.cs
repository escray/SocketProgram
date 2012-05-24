using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public enum ChatType
    {
        Public,
        Private,
    }

    public partial class Main : Form
    {

        /// <summary>
        /// 接收数据缓冲区大小 
        /// </summary>
        private int maxPacket = 1024;
        private Thread receiveThread;

        public NetworkStream Stream { get; set; }
        public string UserName { get; set; }
        public string ServerSocket { get; set; }

        public Main()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        public Main(string userName, string serverSocket, NetworkStream stream) : this()
        {
            UserName = userName;
            ServerSocket = serverSocket;
            Stream = stream;
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确定与服务器断开连接吗?", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.OK)
            {
                byte[] cmd = Encoding.Unicode.GetBytes("cmd::RequestLogout");
                Stream.Write(cmd, 0, cmd.Length);
                if (receiveThread != null)
                {
                    receiveThread.Abort();
                }
                Stream.Close();
                Owner.Close();
                Close();
            }
        }

        // Chat Room
        private void btnChatRoom_Click(object sender, EventArgs e)
        {
            Chat chat = new Chat();
            chat.Stream = Stream;
            chat.UserName = UserName;
            chat.ChatType = ChatType.Public;
            chat.ReceiveName = "PUBLIC";
            chat.Text = "聊天室";
            chat.Show();
        }

        // Talk
        private void btnChat_Click(object sender, EventArgs e)
        {
            

            if (lbxOnline.SelectedItem == null)
            {
                MessageBox.Show("请选择一个接收者！", "发送消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Chat chat = new Chat
                            {
                                Stream = Stream,
                                UserName = UserName,
                                ChatType = ChatType.Private,
                                Text = "私聊",
                                ReceiveName = lbxOnline.SelectedItem.ToString()
                            };

            chat.Show();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Text +=  "\t" + UserName;


        }

        private void ReceiveFreshMessage()
        {
            while (true)
            {
                byte[] bytes = new byte[maxPacket];
                Stream.Read(bytes, 0, bytes.Length);

                if (Encoding.Unicode.GetString(bytes).TrimEnd('\0').Equals("cmd::ResponseList"))
                {
                    byte[] onlineList = new byte[maxPacket];
                    int count = Stream.Read(onlineList, 0, onlineList.Length);
                    IFormatter format = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream();
                    stream.Write(onlineList, 0, count);
                    stream.Position = 0;
                    StringCollection onlineUserList = (StringCollection)format.Deserialize(stream);

                    if (lbxOnline.Items.Count > 0)
                    {
                        lbxOnline.Items.Clear();
                    }
                    
                    foreach (var userName in onlineUserList)
                    {
                        if (!userName.Equals(UserName))
                        {
                            lbxOnline.Items.Add(userName);
                            lbxOnline.SelectedIndex = lbxOnline.Items.Count - 1;
                            lbxOnline.ClearSelected();
                        }
                    }
                }
                else
                {
                    Stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string refreshRequire = "cmd::RequestList";
            Stream.Write(Encoding.Unicode.GetBytes(refreshRequire), 0, Encoding.Unicode.GetBytes(refreshRequire).Length);

            receiveThread = new Thread(ReceiveFreshMessage);
            receiveThread.Start();
        }


    }
}
