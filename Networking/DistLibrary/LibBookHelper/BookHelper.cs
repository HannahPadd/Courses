using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using LibData;

namespace BookHelper
{
    // Note: Do not change this class.
    public class Setting
    {
        public int ServerPortNumber { get; set; }
        public int BookHelperPortNumber { get; set; }
        public int UserHelperPortNumber { get; set; }
        public string ServerIPAddress { get; set; }
        public string BookHelperIPAddress { get; set; }
        public string UserHelperIPAddress { get; set; }
        public int ServerListeningQueue { get; set; }
    }

    // Note: Complete the implementation of this class. You can adjust the structure of this class.
    public class SequentialHelper
    {
        public byte[] buffer;
        public byte[] messageToBeSent;
        public string jsonText;
        public string data;
        public Message message;
        public Message receivedMessage;
        public IPAddress IPAddress;
        public IPEndPoint localEndpoint;
        public SequentialHelper()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
            buffer = new byte[1000];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            localEndpoint = new IPEndPoint(ipAddress, 11112);
        }

        public void start()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(localEndpoint);
            sock.Listen(5);
            Socket serverSock = sock.Accept();
            message = new Message();
            receivedMessage = new Message();
            messageToBeSent = new byte[1000];

            while (true)
            {

            }
        }
    }
}
