using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using LibData;
using System.Text;

namespace LibServer
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
    public class SequentialServer
    {
        public byte[] buffer; 
        public byte[] messageToBeSent;
        public string jsonText; 
        public string data;
        public Message receivedMessage;
        public IPAddress IPAddress;
        public IPEndPoint localEndpoint;

        public SequentialServer()
        {
            //todo: implement the body. Add extra fields and methods to the class if it is needed
            buffer = new byte[1000];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            localEndpoint = new IPEndPoint(ipAddress, 11111);
        }

        public void start()
        {
            //todo: implement the body. Add extra fields and methods to the class if it is needed

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(localEndpoint);
            sock.Listen(5);
            Socket serverSock = sock.Accept();
            Message message = new Message();
            message = new Message();
            receivedMessage = new Message();
            messageToBeSent = new byte[1000];

            while (true)
            {
                try
                {
                    int b = serverSock.Receive(buffer);
                    data = Encoding.ASCII.GetString(buffer, 0, b);
                    receivedMessage = JsonSerializer.Deserialize<Message>(data);
                }
                catch
                {
                    Console.WriteLine("waiting for a new message");
                }
                switch (receivedMessage.Type)
                {
                    case MessageType.Hello:
                        Console.WriteLine("recieved Hello");
                        message.Type = (MessageType)1;
                        jsonText = JsonSerializer.Serialize<Message>(message);
                        messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                        Console.WriteLine("returned welcome");
                        serverSock.Send(messageToBeSent);
                        break;

                    case MessageType.Welcome:
                        break;

                   case MessageType.BookInquiry:
                        Console.WriteLine("recieved bookInquiry");
                        Console.WriteLine("sending bookInquiry to helper");
                        receivedMessage = JsonSerializer.Deserialize<Message>(Bookhelper(receivedMessage));
                        jsonText = JsonSerializer.Serialize<Message>(receivedMessage);
                        messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                        break;


                 case MessageType.UserInquiry:
                        Console.WriteLine("recieved UserInquiry");
                        Console.WriteLine("sending UserInquiry to helper");
                        receivedMessage = JsonSerializer.Deserialize<Message>(Userhelper(receivedMessage));
                        jsonText = JsonSerializer.Serialize<Message>(receivedMessage);
                        messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                        break;

                    case MessageType.BookInquiryReply:
                        Console.WriteLine("Received bookinquiry");
                        ConnectToBookHelper();
                        jsonText = JsonSerializer.Serialize<Message>(receivedMessage);
                        messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                        
                        Console.WriteLine("book has been send to client");
                        break;


                    case MessageType.UserInquiryReply:
                        break;

                    case MessageType.EndCommunication:
                        sock.Close();
                        break;

                    case MessageType.Error:
                        break;

                    case MessageType.NotFound:
                        break;

                }

                    Console.WriteLine("" + data);
                    Console.WriteLine("" + jsonText);
                    data = null;
                    serverSock.Send(messageToBeSent);
            }

            void ConnectToBookHelper()
            {
                    Socket bookHelperSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint bookHelperEndpoint = new IPEndPoint(IPAddress, 11112);
                    bookHelperSock.Bind(bookHelperEndpoint);

            }

        }
        public string Bookhelper(Message recieved)
        {
            byte[] buffer = new byte[10000];
            byte[] msg = new byte[1000];
            string data = null;
            byte[] clientInfo = new byte[1000];
            string Jsonmsg = JsonSerializer.Serialize<Message>(recieved);
            clientInfo = Encoding.ASCII.GetBytes(Jsonmsg);
            Message receivedMessage = new Message();

            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint serverEndpoint = new IPEndPoint(iPAddress, 11112);
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(serverEndpoint);

            sock.Send(clientInfo);
            int c = sock.Receive(buffer);
            data = Encoding.ASCII.GetString(buffer, 0, c);
            Console.WriteLine(data);

            return data;


        }
        public string Userhelper(Message recieved)
        {
            byte[] buffer = new byte[10000];
            byte[] msg = new byte[1000];
            string data = null;
            byte[] clientInfo = new byte[1000];
            string Jsonmsg = JsonSerializer.Serialize<Message>(recieved);
            clientInfo = Encoding.ASCII.GetBytes(Jsonmsg);
            Message receivedMessage = new Message();

            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint serverEndpoint = new IPEndPoint(iPAddress, 11113);
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(serverEndpoint);

            sock.Send(clientInfo);
            int c = sock.Receive(buffer);
            data = Encoding.ASCII.GetString(buffer, 0, c);

            return data;


        }
    }
}



