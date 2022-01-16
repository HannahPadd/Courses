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
        bool newmessage = false;
        public Socket BookHelperSock;
        public Socket UserHelperSock;
        public bool server = true;
        public bool endCom = false;


        public SequentialServer()
        {
            //todo: implement the body. Add extra fields and methods to the class if it is needed
            buffer = new byte[1000];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            localEndpoint = new IPEndPoint(ipAddress, 11111);
        }

        public virtual void start()
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

            while (server)
            {
                try
                {
                    int b = serverSock.Receive(buffer);
                    data = Encoding.ASCII.GetString(buffer, 0, b);
                    receivedMessage = JsonSerializer.Deserialize<Message>(data);
                    newmessage = true;
                }
                catch
                {
                    
                    sock.Listen(5);
                    serverSock = sock.Accept();
                }
                if (newmessage)
                {
                    switch (receivedMessage.Type)
                    {
                        case MessageType.Hello:
                            
                            message.Type = (MessageType)1;
                            jsonText = JsonSerializer.Serialize<Message>(message);
                            messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                            
                            serverSock.Send(messageToBeSent);
                            newmessage = false;
                            break;


                        case MessageType.BookInquiry:
                            
                            receivedMessage = JsonSerializer.Deserialize<Message>(Bookhelper(receivedMessage));
                            jsonText = JsonSerializer.Serialize<Message>(receivedMessage);
                            messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                            serverSock.Send(messageToBeSent);
                            
                            newmessage = false;
                            break;


                        case MessageType.UserInquiry:
                            
                            receivedMessage = JsonSerializer.Deserialize<Message>(Userhelper(receivedMessage));
                            jsonText = JsonSerializer.Serialize<Message>(receivedMessage);
                            messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                            serverSock.Send(messageToBeSent);
                            newmessage = false;
                            break;

                        case MessageType.EndCommunication:
                            endCom = true;
                            Bookhelper(receivedMessage);
                            Userhelper(receivedMessage);
                            server = false;
                            sock.Close();
                            break;


                    }

                    
                    data = null;
                }
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

            if (BookHelperSock == null)
            {
                IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint serverEndpoint = new IPEndPoint(iPAddress, 11112);
                BookHelperSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                BookHelperSock.Connect(serverEndpoint);
            }
            if (endCom == true)
            {
                BookHelperSock.Send(clientInfo);
                return "Connection is ended";
            }
            else {
                BookHelperSock.Send(clientInfo);
                int c = BookHelperSock.Receive(buffer);
                
                data = Encoding.ASCII.GetString(buffer, 0, c);
                

                return data;
            }


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

            if (UserHelperSock == null)
            {
                IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint serverEndpoint = new IPEndPoint(iPAddress, 11113);
                UserHelperSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                UserHelperSock.Connect(serverEndpoint);
            }

            if (endCom == true)
            {
                UserHelperSock.Send(clientInfo);
                return "Connection is ended";
            }
            UserHelperSock.Send(clientInfo);
            int c = UserHelperSock.Receive(buffer);
            
            data = Encoding.ASCII.GetString(buffer, 0, c);

            return data;


        }
    }
}



