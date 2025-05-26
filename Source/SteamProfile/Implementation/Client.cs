using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace SteamProfile.Implementation
{
    public partial class Client
    {
        /*
        private IPEndPoint serverEndPoint;
        private Socket clientSocket;
        private DispatcherQueue uiThread;
        private Regex infoChangeCommandRegex;

        private ClientStatus clientStatus;

        public event EventHandler<MessageEventArgs> NewMessageReceivedEvent;
        public event EventHandler<ClientStatusEventArgs> ClientStatusChangedEvent;

        private string userName;
        private string infoChangeCommandPattern;

        /// <summary>
        /// Creates the client component used to connect to a server
        /// </summary>
        /// <param name="hostIpAddress">The ip address of the user who sent the invite</param>
        /// <param name="userName">The name of the user who joined the chat room</param>
        /// <param name="uiThread">Updating the ui can be done using only the main thread</param>
        /// <exception cref="Exception">Ip address of the server provided in the wrong format</exception>
        public Client(string hostIpAddress, string userName, DispatcherQueue uiThread)
        {
            this.userName = userName;
            // A client will only receive valid commands in the form "<INFO>|Status|<INFO>"
            // Once he receives that status, he will change the value to it's negation (admin -> !admin) from the client status
            this.infoChangeCommandPattern = @"^<INFO>\|.*\|<INFO>$";
            this.uiThread = uiThread;

            this.clientStatus = new ClientStatus();

            // Command matches are found via regex
            this.infoChangeCommandRegex = new Regex(this.infoChangeCommandPattern);

            try
            {
                this.serverEndPoint = new IPEndPoint(IPAddress.Parse(hostIpAddress), Server.PORT_NUMBER);
                this.clientSocket = new(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task ConnectToServer()
        {
            try
            {
                // Initialize connection to the server
                await clientSocket.ConnectAsync(serverEndPoint);

                // The client sends his username on first connection to be identified later on
                byte[] userNameToBytes = Encoding.UTF8.GetBytes(userName);
                _ = await clientSocket.SendAsync(userNameToBytes, SocketFlags.None);

                // Create a new "promise", which we assign the "ReceiveMessage"
                // Task.Run() will create a new "thread" in thread pool which will work until the server closes
                _ = Task.Run(() => ReceiveMessage());

                this.clientStatus.IsConnected = true;

                // If this client is host, the service will set the value to true, but only after creating
                // the client, so we update the client status to the ui
                this.uiThread.TryEnqueue(() => this.ClientStatusChangedEvent?.Invoke(this, new ClientStatusEventArgs(clientStatus)));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task SendMessageToServer(string message)
        {
            try
            {
                byte[] messageToBytes = Encoding.UTF8.GetBytes(message);

                // Sends the message to the server
                _ = await clientSocket.SendAsync(messageToBytes, SocketFlags.None);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        private async Task ReceiveMessage()
        {
            try
            {
                while (true)
                {
                    // We allocate more bytes than needed (around 4000) to process a message in one go
                    byte[] messageBuffer = new byte[Server.MESSAGE_MAXIMUM_SIZE];
                    int messageLength = await clientSocket.ReceiveAsync(messageBuffer, SocketFlags.None);

                    // If we received 0, means the server closed it's connection
                    if (messageLength == Server.DISCONNECT_CODE)
                    {
                        break;
                    }

                    // Message is a Google protobuf auto generated class
                    // Check the "message.proto" for it's attributes (properties are auto generated as well)
                    Message message = Message.Parser.ParseFrom(messageBuffer, 0, messageLength);
                    // Check if the message content is a "info" command (a status change)
                    if (this.infoChangeCommandRegex.IsMatch(message.MessageContent))
                    {
                        // We are looking for the status that is between the | | in "<INFO>|Status|<INFO>
                        int newInfoIndex = 1;
                        char commandSeparator = '|';
                        string newInfo = message.MessageContent.Split(commandSeparator)[newInfoIndex];

                        this.UpdateClientStatus(newInfo);
                        continue;
                    }

                    this.uiThread.TryEnqueue(() => NewMessageReceivedEvent?.Invoke(this, new MessageEventArgs(message)));
                }
            }
            catch (Exception)
            {
                // An exception would mean something went wrong on the server,
                // hence we close the connection and update the IsConnected property,
                // to allow the main thread to throw errors, errors from here won't be catched
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public bool IsConnected()
        {
            return this.clientStatus.IsConnected;
        }

        private void UpdateClientStatus(string newStatus)
        {
            switch (newStatus)
            {
                case Server.ADMIN_STATUS:
                    this.clientStatus.IsAdmin = !this.clientStatus.IsAdmin;
                    break;
                case Server.MUTE_STATUS:
                    this.clientStatus.IsMuted = !this.clientStatus.IsMuted;
                    break;
                case Server.KICK_STATUS:
                    this.CloseConnection();
                    break;
                default:
                    // In case we receive a wrong command
                    break;
            }

            // Signal the UI to update the status for the client
            this.uiThread.TryEnqueue(() => this.ClientStatusChangedEvent?.Invoke(this, new ClientStatusEventArgs(clientStatus)));
        }

        public void SetIsHost()
        {
            this.clientStatus.IsHost = true;
        }

        public async void Disconnect()
        {
            try
            {
                // Send an empty message representing a disconnect
                _ = await clientSocket.SendAsync(new byte[Server.DISCONNECT_CODE], SocketFlags.None);
                this.CloseConnection();
            }
            catch (Exception)
            {
                // Ignore the exception (can happen if the client already disconnected)
            }
        }

        private void CloseConnection()
        {
            this.clientStatus.IsConnected = false;
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
        */
    }
}
