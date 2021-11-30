﻿using System;
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
        private string bookData = @"Books.json";
        private List<BookData> bookDataList;
        private BookData[] books;
        public SequentialHelper()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
            buffer = new byte[1000];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            localEndpoint = new IPEndPoint(ipAddress, 11112);
        }

        public void start()
        {
            string BookContent = File.ReadAllText(bookData);
            this.bookDataList = JsonSerializer.Deserialize<List<BookData>>(BookContent);
            books = new BookData[this.bookDataList.Count];

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
                Console.WriteLine("Succesfully connected");

                switch (receivedMessage.Type)
                {
                    case MessageType.Hello:
                        break;

                    case MessageType.Welcome:

                        sock.Send(messageToBeSent);
                        break;

                    case MessageType.BookInquiry:
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
                                Console.WriteLine("book info send to server");
                                serverSock.Send(messageToBeSent);
                                break;
                            }
                        }
                        break;

                    case MessageType.UserInquiry:
                        break;

                    case MessageType.BookInquiryReply:
                        break;

                    case MessageType.UserInquiryReply:
                        break;


                    case MessageType.EndCommunication:
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