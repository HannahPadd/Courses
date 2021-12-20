using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using LibData;
using System.Text;

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
        private string bookData = @"C:\Users\giova\OneDrive\Bureaublad\Courses\Networking\DistLibrary\LibBookHelper\Books.json";
        private List<BookData> bookDataList;
        private BookData[] books;
        public bool bookfound = false;
        public bool server = true;
       

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
            string BookContent = File.ReadAllText(bookData);
            this.bookDataList = JsonSerializer.Deserialize<List<BookData>>(BookContent);
            books = new BookData[this.bookDataList.Count];
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
                        bookfound = false;
                        Console.WriteLine("recieved bookInquiry");
                        foreach (BookData d in this.bookDataList)
                        {
                            if (d.Title == receivedMessage.Content)
                            {
                                Console.WriteLine(d);
                                message.Content = JsonSerializer.Serialize<BookData>(d);
                                message.Type = (MessageType)4;
                                jsonText = JsonSerializer.Serialize<Message>(message);
                                messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                                serverSock.Send(messageToBeSent);
                                Console.WriteLine("book info send to server");
                                bookfound = true;
                                break;
                            }
                        }
                        if (!bookfound)
                        {
                            message = new Message();
                            message.Type = (MessageType)8;
                            jsonText = JsonSerializer.Serialize<Message>(message);
                            messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                            serverSock.Send(messageToBeSent);
                            Console.WriteLine("book was not found");
                        }
                        break;

                    case MessageType.UserInquiry:
                        break;

                    case MessageType.BookInquiryReply:

                        break;

                    case MessageType.UserInquiryReply:
                        break;

                    case MessageType.EndCommunication:
                        server = false;
                        sock.Close();
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
