using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using LibData;
using System.Text;

namespace UserHelper
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
        private string userData = @"Users.json";
        private List<UserData> userDataList;
        private UserData[] users;
        public bool server = true;

        public SequentialHelper()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
            buffer = new byte[1000];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            localEndpoint = new IPEndPoint(ipAddress, 11113);
        }

        public void start()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
            string UserContent = File.ReadAllText(userData);
            this.userDataList = JsonSerializer.Deserialize<List<UserData>>(UserContent);
            users = new UserData[this.userDataList.Count];

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(localEndpoint);
            sock.Listen(5);
            Socket serverSock = sock.Accept();
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
                    Console.WriteLine(receivedMessage.Type);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("[Server exception] {0}", e.Message);
                }
                switch (receivedMessage.Type)
                {
                    case MessageType.Hello:

                        break;

                    case MessageType.Welcome:
                        break;

                    case MessageType.BookInquiry:
                        break;

                    case MessageType.UserInquiry:
                        Console.WriteLine("recieved bookInquiry");
                        foreach (UserData d in this.userDataList)
                        {
                            if (d.User_id == receivedMessage.Content)
                            {
                                Console.WriteLine(d);
                                message.Content = JsonSerializer.Serialize<UserData>(d);
                                message.Type = MessageType.UserInquiryReply;
                                jsonText = JsonSerializer.Serialize<Message>(message);
                                messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                                serverSock.Send(messageToBeSent);
                                Console.WriteLine("user info send to server");
                                break;
                            }
                        }
                        break;

                    case MessageType.BookInquiryReply:

                        break;

                    case MessageType.UserInquiryReply:
                        break;

                    case MessageType.EndCommunication:
                        server = false;
                        sock.Close();
                        Console.WriteLine("End communication.");
                        break;

                    case MessageType.Error:
                        break;

                    case MessageType.NotFound:
                        break;

                }
            
            }
        }
    }
}
