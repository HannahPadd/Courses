using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using LibData;
using System.Text;

namespace LibClient
{
    // Note: Do not change this class 
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

    // Note: Do not change this class 
    public class Output
    {
        public string Client_id { get; set; } // the id of the client that requests the book
        public string BookName { get; set; } // the name of the book to be reqyested
        public string Status { get; set; } // final status received from the server
        public string BorrowerName { get; set; } // the name of the borrower in case the status is borrowed, otherwise null
        public string BorrowerEmail { get; set; } // the email of the borrower in case the status is borrowed, otherwise null
    }

    // Note: Complete the implementation of this class. You can adjust the structure of this class.
    public class SimpleClient
    {
        // some of the fields are defined. 
        public Output result;
        public Socket clientSocket;
        public IPEndPoint serverEndPoint;
        public IPAddress ipAddress;
        public Setting settings;
        public string client_id;
        private string bookName;
        // all the required settings are provided in this file
        //public string configFile = @"../ClientServerConfig.json";
        public string configFile = @"../../../../ClientServerConfig.json"; // for debugging

        // todo: add extra fields here in case needed 
        public byte[] buffer;
        public byte[] messageToBeSent;
        public Message message;
        public Message receivedMessage;
        public string data;
        public string jsonText;
        bool newmessage = false;
        /// <summary>
        /// Initializes the client based on the given parameters and seeting file.
        /// </summary>
        /// <param name="id">id of the clients provided by the simulator</param>
        /// <param name="bookName">name of the book to be requested from the server, provided by the simulator</param>
        public SimpleClient(int id, string bookName)
        {
            //todo: extend the body if needed.
            this.bookName = bookName;
            this.client_id = "Client " + id.ToString();
            this.result = new Output();
            result.BookName = bookName;
            result.Client_id = this.client_id;
            buffer = new byte[1000];

            // read JSON directly from a file
            try
            {
                string configContent = File.ReadAllText(configFile);
                this.settings = JsonSerializer.Deserialize<Setting>(configContent);
                this.ipAddress = IPAddress.Parse(settings.ServerIPAddress);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }
            this.serverEndPoint = new IPEndPoint(this.ipAddress, 11111);
        }

        /// <summary>
        /// Establishes the connection with the server and requests the book according to the specified protocol.
        /// Note: The signature of this method must not change.
        /// </summary>
        /// <returns>The result of the request</returns>
        public Output start()
        {

            // todo: implement the body to communicate with the server and requests the book. Return the result as an Output object.
            // Adding extra methods to the class is permitted. The signature of this method must not change.
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(this.serverEndPoint);

            message = new Message();
            receivedMessage = new Message();
            messageToBeSent = new byte[1000];
            message.Type = MessageType.Hello;
            message.Content = client_id;
            string jsonText = JsonSerializer.Serialize<Message>(message);
            messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
            sock.Send(messageToBeSent);
            Console.WriteLine("Succesfully connected");


            while (true)
            {
               
                try {
                    int b = sock.Receive(buffer);
                    data = Encoding.ASCII.GetString(buffer, 0, b);
                    receivedMessage = JsonSerializer.Deserialize<Message>(data);
                    newmessage = true;
                }
                catch 
                { 
                    Console.WriteLine("waiting for a new message"); 
                }

                
                if(newmessage) {
                    switch (receivedMessage.Type)
                    {
                        case MessageType.Hello:
                            break;

                        case MessageType.Welcome:
                            if (Convert.ToInt32(client_id.Replace("Client ", "")) == -1)
                            {
                                Console.WriteLine("Ending comunications");
                                message = new Message();
                                message.Type = MessageType.EndCommunication;
                                jsonText = JsonSerializer.Serialize<Message>(message);
                                messageToBeSent = Encoding.ASCII.GetBytes(jsonText);

                                sock.Send(messageToBeSent);
                                sock.Close();
                                return result;
                            }
                            else
                            {
                                Console.WriteLine("sending bookInquiry");
                                message = new Message();
                                message.Type = (MessageType)2;
                                message.Content = bookName;
                                jsonText = JsonSerializer.Serialize<Message>(message);
                                messageToBeSent = Encoding.ASCII.GetBytes(jsonText);

                                sock.Send(messageToBeSent);
                                newmessage = false;
                            }
                            break;

                        case MessageType.BookInquiry:
                            Console.WriteLine("Sening book inquiry");
                            message.Type = MessageType.BookInquiry;
                            message.Content = bookName;
                            jsonText = JsonSerializer.Serialize<Message>(message);
                            messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                            sock.Send(messageToBeSent);
                            newmessage = false;
                            break;

                        case MessageType.UserInquiry:
                            break;

                        case MessageType.BookInquiryReply:
                            Console.WriteLine("got book back");
                            BookData book = JsonSerializer.Deserialize<BookData>(receivedMessage.Content);
                            this.result.Status = book.Status;
                            if (book.Status == "Borrowed")
                            {
                                string userId = book.BorrowedBy;
                                message = new Message();
                                message.Type = MessageType.UserInquiry;
                                message.Content = userId;
                                jsonText = JsonSerializer.Serialize<Message>(message);
                                messageToBeSent = Encoding.ASCII.GetBytes(jsonText);
                                sock.Send(messageToBeSent);
                                newmessage = false;
                            }
                            else
                            {
                                sock.Close();
                                return result;
                                
                            }
                            break;

                        case MessageType.UserInquiryReply:
                            Console.WriteLine("got user back");
                            UserData user = JsonSerializer.Deserialize<UserData>(receivedMessage.Content);
                            this.result.BorrowerName = user.Name;
                            this.result.BorrowerEmail = user.Email;
                            sock.Close();
                            return result;


                        case MessageType.EndCommunication:
                            break;

                        case MessageType.Error:
                            break;

                        case MessageType.NotFound:
                            sock.Close();
                            return result;
                            
                    }
                }
                
            }

        }
    }
}
