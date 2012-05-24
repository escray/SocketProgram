using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {
        /// <summary>
        /// 服务器端的监听器
        /// </summary>
        private TcpListener tcpListener;
        /// <summary>
        /// 服务器程序使用的端口，默认为11000
        /// </summary>
        private int port = 11000;
        /// <summary>
        /// 接收数据缓冲区大小 
        /// </summary>
        private int maxPacket = 1024;
        /// <summary>
        /// 保存所有客户端会话的字典表
        /// </summary>
        private Dictionary<string, Socket> allUser = new Dictionary<string, Socket>(); 

        public Server()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            TcpListenerStart();
            //SocketListener();
        }

        private string userName = string.Empty;

        public void TcpListenerStart()
        {
            tcpListener = new TcpListener(GetServerIPAddress(), port);
            
            tcpListener.Start();
            Display("服务器启动，开始监听......");

            var connThread = new Thread(SocketListenerConnect);
            connThread.Start();
        }

        private void SocketListenerConnect()
        {
            while (true)
            {
                var bytes = new byte[maxPacket];
                try
                {
                    var socket = tcpListener.AcceptSocket();
                    //var socket = client.Client;
                    socket.Receive(bytes);
                    userName = Encoding.Unicode.GetString(bytes).TrimEnd('\0');

                    // debug
                    Display("send: " + userName);

                    if (allUser.Count != 0 && allUser.ContainsKey(userName))
                    {
                        socket.Send(Encoding.Unicode.GetBytes("cmd::Failed"));
                        continue;
                    }

                    socket.Send(Encoding.Unicode.GetBytes("cmd::Successful"));

                    allUser.Add(userName, socket);

                    string log = string.Format("[系统消息]新用户 {0} 在 {1} 已连接，当前在线人数： {2}", userName, DateTime.Now, allUser.Count);

                    
                    Display(log);
                    SendMessageToAll(userName, log);
                    var clientThread = new Thread(MessageTransfer);
                    clientThread.Start(userName);

                }
                catch (Exception)
                {
                    
                    
                }
            }
            // AcceptTcpClient()
            //var client = tcpListener.AcceptTcpClient();
            
            
        }

        private void SendMessageToAll(string userName, string log)
        {
            foreach (var pair in allUser.Where(pair => pair.Key != userName))
            {
                //pair.Value.Send(Encoding.Unicode.GetBytes(log));
                pair.Value.Send(Encoding.Unicode.GetBytes(log));
            }
        }

        private void MessageTransfer(object obj)
        {
            string key = obj.ToString();
            if (!allUser.ContainsKey(key))
            {
                return;
            }
            Socket clientSocket = allUser[key];
            while (true)
            {
                try
                {
                    // receive first datagram
                    byte[]  bytes = new byte[maxPacket];
                    clientSocket.Receive(bytes);
                    string receive = Encoding.Unicode.GetString(bytes).TrimEnd('\0');
                    
                    Display("first datagram: " + receive);

                    if (receive.StartsWith("cmd::RequestLogout"))
                    {
                        allUser.Remove(key);
                        string log = string.Format("[系统消息]用户 {0} 在 {1} 已断开。当前在线人数：{2}", key, DateTime.Now, allUser.Count);
                        
                        Display(log);

                        SendMessageToAll(key, log);
                        Thread.CurrentThread.Abort();
                    }
                    else if (receive.StartsWith("cmd::RequestList"))
                    {
                        var onlineUser = SerializeOnlineUserList();
                        clientSocket.Send(Encoding.Unicode.GetBytes("cmd::ResponseList"));
                        clientSocket.Send(onlineUser);
                        continue;
                    }
                    else if (receive.StartsWith("cmd::BroadCast"))
                    {
                        clientSocket.Receive(bytes);
                        string broadcastMsg = Encoding.Unicode.GetString(bytes);

                        Display("BroadCast: " + broadcastMsg);

                        SendMessageToAll(key, broadcastMsg);
                        continue;
                    }
                    // second datagram
                    // here has a question fro transfer message 2012-5-24 12:14
                    clientSocket.Receive(bytes);
                    //debug
                    Display("second datagram: " + Encoding.Unicode.GetString(bytes));
                    
                    try
                    {
                        // 转发
                        var receiveSocket = allUser[receive];
                        //if (receiveSocket != null && receiveSocket.Connected)
                        //{
                            //clientSocket.Send(bytes);
                            receiveSocket.Send(bytes);
                        //}
                    }
                    catch (Exception)
                    {
                        string errorMessage = string.Format("[系统消息]您刚才的内容没有发送成功。用户：{0} 已离线或者网络阻塞。", receive);
                        clientSocket.Send(Encoding.Unicode.GetBytes(errorMessage));
                    }
                }
                catch (SocketException)
                {
                    if (allUser.ContainsKey(key))
                    {
                        allUser.Remove(key);
                    }
                    string log = string.Format("[系统消息]用户 {0} 的客户端在 {1} 意外终止！当前在线人数：{2}", key, DateTime.Now, allUser.Count);
                    Display("ErrorLog: " + log);
                    SendMessageToAll(string.Empty, log);
                    Thread.CurrentThread.Abort();
                }
            }
        }

        private byte[] SerializeOnlineUserList()
        {
            byte[] bytes;
            var onlineUserList = new StringCollection();
            foreach (var pair in allUser)
            {
                onlineUserList.Add(pair.Key);
            }
            IFormatter format = new BinaryFormatter();
            using(var stream  = new MemoryStream())
            {
                format.Serialize(stream, onlineUserList);
                bytes = stream.ToArray();
                stream.Close();
                return bytes;
            }
        }

        private void CloseTcpListener()
        {
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }
            
            if (allUser.Count == 0) return;

            foreach (var client in allUser.Values)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            allUser.Clear();
            allUser = null;
        }

        private void SocketListener()
        {
            string data = null;
            var bytes = new byte[maxPacket];

            var ipAddress = GetServerIPAddress();
            var serverEndPoint = new IPEndPoint(ipAddress, port);
            // Create a TCP/IP socket
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket handler = null;


            // Bind the socket to the local endpoint and listen for incoming connection
            try
            {
                listener.Bind(serverEndPoint);
                listener.Listen(32);
                Display("等待连接......");
                // Program is suspended while waiting for a incoming connection
                handler = listener.Accept();
                // Start listening for connections
                while (true)
                {
                    data = null;
                    // An incoming connection needs to be processed
                    while (true)
                    {
                        bytes = new byte[maxPacket];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>", StringComparison.Ordinal) > -1)
                        {
                            break;
                        }
                    }
                    Display(string.Format("Text received: {0}", data));
                    // Echo the data back to the client
                    byte[] msg = Encoding.ASCII.GetBytes(data);
                    handler.Send(msg);
                }
            }
            catch (Exception ee)
            {
                Display(ee.Message);
            }
            finally
            {
                if (handler == null)
                {
                }
                else
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
        }

        private delegate void DisplayDelegate(string msg);

        /// <summary>
        /// 在 ListBox 中追加状态信息
        /// </summary>
        /// <param name="message">message of append</param>
        private void Display(string message)
        {
            if (lbxServerDisplay.InvokeRequired)
            {
                DisplayDelegate d = Display;
                lbxServerDisplay.Invoke(d, message);
            }
            else
            {
                lbxServerDisplay.Items.Add(message);
                lbxServerDisplay.SelectedIndex = lbxServerDisplay.Items.Count - 1;
                lbxServerDisplay.ClearSelected();
            }

            lbxServerDisplay.SelectedIndex = lbxServerDisplay.Items.Count - 1;
            lbxServerDisplay.ClearSelected();
            //lbxServerDisplay.Items.Add(message);
        }

        private static IPAddress GetServerIPAddress()
        {
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            // 127.0.0.1 => 16777343
            return ipAddress ?? new IPAddress(16777343);
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            CloseTcpListener();
            Close();
        }

        private void Server_Load(object sender, EventArgs e)
        {

        }
    }
}
