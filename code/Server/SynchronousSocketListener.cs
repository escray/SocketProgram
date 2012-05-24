using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public class SynchronousSocketListener
    {

        // Incoming data from the client.
        public static string Data;
        public static Control DisplayControl;
        
        static void Display(string message)
        {
            var control = DisplayControl as ListBox;

            if(control != null)
            {
                control.Items.Add(message);
            }
        }

        public static void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes;

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            var ipHostInfo = Dns.Resolve(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.
                while (true)
                {
                    //Console.WriteLine(@"Waiting for a connection...");
                    Display(@"Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.
                    Socket handler = listener.Accept();
                    Data = null;

                    // An incoming connection needs to be processed.
                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        Data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (Data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    // Show the data on the console.
                    //Console.WriteLine("Text received : {0}", Data);
                    Display(string.Format("Text received : {0}", Data));
                    // Echo the data back to the client.
                    byte[] msg = Encoding.ASCII.GetBytes(Data);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //Console.WriteLine("\nPress ENTER to continue...");
            Display("nPress ENTER to continue...");

        }

        public static void Initial(ListBox listbox)
        {
            DisplayControl = listbox;
        }
    }
}
