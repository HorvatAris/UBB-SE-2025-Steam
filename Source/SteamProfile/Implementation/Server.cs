using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Protobuf;

namespace SteamProfile.Implementation
{
    public partial class Server
    {
        /*
        private Socket serverSocket;
        private IPEndPoint ipEndPoint;
        private System.Threading.Timer? serverTimeout;
        private readonly object lockTimer;

        private ConcurrentDictionary<string, string> addressesAndUserNames;
        private ConcurrentDictionary<Socket, string> socketsAndAddresses;
        private ConcurrentDictionary<string, Socket> userNamesAndSockets;
        private ConcurrentDictionary<string, bool> mutedUsers;
        private ConcurrentDictionary<string, bool> adminUsers;

        private Regex muteCommandRegex;
        private Regex adminCommandRegex;
        private Regex kickCommandRegex;
        private Regex infoChangeCommandRegex;

        private string hostName;
        private string muteCommandPattern;
        private string adminCommandPattern;
        private string kickCommandPattern;
        private string infoCommandPattern;

        private bool isRunning;

        // Port number is always the same
        public const int PORT_NUMBER = 6000;

        public const int MESSAGE_MAXIMUM_SIZE = 4112;
        public const int USER_NAME_MAXIMUM_SIZE = 512;
        public const int MAXIMUN_NUMBER_OF_ACTIVE_CONNECTIONS = 20;
        public const int NUMBER_OF_QUEUED_CONNECTIONS = 10;
        public const int STARTING_INDEX = 0;
        public const int DISCONNECT_CODE = 0;
        public const int SERVER_TIMEOUT_COUNTDOWN = 180000;
        public const int MINIMUM_CONNECTIONS = 2;
        public const char ADDRESS_SEPARATOR = ':';
        public const string ADMIN_STATUS = "ADMIN";
        public const string MUTE_STATUS = "MUTE";
        public const string KICK_STATUS = "KICK";
        public const string HOST_STATUS = "HOST";
        public const string REGULAR_USER_STATUS = "USER";
        public const string INFO_CHANGE_MUTE_STATUS_COMMAND = "<INFO>|" + MUTE_STATUS + "|<INFO>";
        public const string INFO_CHANGE_ADMIN_STATUS_COMMAND = "<INFO>|" + ADMIN_STATUS + "|<INFO>";
        public const string INFO_CHANGE_KICK_STATUS_COMMAND = "<INFO>|" + KICK_STATUS + "|<INFO>";
        public const string SERVER_REJECT_COMMAND = "Server rejected your command!\n You don't have the rights to that user!";
        public const string SERVER_CAPACITY_REACHED = "Server capacity reached!\n Closing Connection!";

        public Server(string hostAddress, string hostName)
        {
            // Info commands change a user status, they are sent by the server after receiving
            // a mute/admin/kick command. They follow the following pattern: "<nameOfTheInvoker>|targetedStatus|<nameOfTheTargetedUser>"
            this.muteCommandPattern = @"^<.*>\|" + Server.MUTE_STATUS + @"\|<.*>$";
            this.adminCommandPattern = @"^<.*>\|" + Server.ADMIN_STATUS + @"\|<.*>$";
            this.kickCommandPattern = @"^<.*>\|" + Server.KICK_STATUS + @"\|<.*>$";
            this.infoCommandPattern = @"^<INFO>\|.*\|<INFO>$";

            this.muteCommandRegex = new Regex(this.muteCommandPattern);
            this.adminCommandRegex = new Regex(this.adminCommandPattern);
            this.kickCommandRegex = new Regex(this.kickCommandPattern);
            this.infoChangeCommandRegex = new Regex(this.infoCommandPattern);

            this.addressesAndUserNames = new ConcurrentDictionary<string, string>();
            this.socketsAndAddresses = new ConcurrentDictionary<Socket, string>();
            this.userNamesAndSockets = new ConcurrentDictionary<string, Socket>();
            this.mutedUsers = new ConcurrentDictionary<string, bool>();
            this.adminUsers = new ConcurrentDictionary<string, bool>();

            // The data structures used for storing informations about the users are thread safe
            // but the server will also work with a timeout, so we use a lock to guarantee safety
            this.lockTimer = new object();

            this.hostName = hostName;

            try
            {
                this.ipEndPoint = new IPEndPoint(IPAddress.Parse(hostAddress), Server.PORT_NUMBER);
                this.serverSocket = new(this.ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                this.serverSocket.Bind(this.ipEndPoint);
                this.serverSocket.Listen(Server.NUMBER_OF_QUEUED_CONNECTIONS);
            }
            catch (Exception exception)
            {
                throw new Exception($"Server create error: {exception.Message}");
            }

            this.isRunning = true;
        }

        public async void Start()
        {
            // Handle new connection until the server is no longer running
            while (this.isRunning)
            {
                try
                {
                    // Accepts a new connection
                    Socket clientSocket = await this.serverSocket.AcceptAsync();

                    string endPointNullResult = "Not Connected";

                    // RemoteEndPoint provides us with a string of form: "ip_address:port"
                    string ipAddressAndPort = clientSocket.RemoteEndPoint?.ToString() ?? endPointNullResult;

                    // If the remote end point was null, this will throw an error from the IndexOf call
                    string ipAddress = ipAddressAndPort.Substring(Server.STARTING_INDEX, ipAddressAndPort.IndexOf(Server.ADDRESS_SEPARATOR));

                    // Server has a limit of 20 active users per requirements
                    if (this.socketsAndAddresses.Count >= Server.MAXIMUN_NUMBER_OF_ACTIVE_CONNECTIONS)
                    {
                        // Notify the user and then "Kick" the user (close forcefully the connection)
                        this.SendMessageToOneClient(CreateMessage(Server.SERVER_CAPACITY_REACHED, this.hostName), clientSocket);
                        this.SendMessageToOneClient(CreateMessage(Server.INFO_CHANGE_KICK_STATUS_COMMAND, this.hostName), clientSocket);
                        continue;
                    }

                    // Store the user data available for now
                    this.socketsAndAddresses.TryAdd(clientSocket, ipAddress);

                    // Check if the minimum connections, if so, initializes the timeout (this happens only when the host joins)
                    this.StartTimeoutIfMinimumConnectionsNotMet();

                    // A new "thread" is created to handle receiving messages / commands from clients
                    _ = Task.Run(() => HandleClient(clientSocket));
                }
                catch (Exception)
                {
                    // No error thrown back, if we do we stop the server
                }
            }
        }

        private async Task HandleClient(Socket clientSocket)
        {
            try
            {
                // Receive the username at the beginning of connection
                byte[] userNameBuffer = new byte[Server.USER_NAME_MAXIMUM_SIZE];
                int userNameLength = await clientSocket.ReceiveAsync(userNameBuffer, SocketFlags.None);

                string userName = Encoding.UTF8.GetString(userNameBuffer, Server.STARTING_INDEX, userNameLength);
                string ipAddress = this.socketsAndAddresses.GetValueOrDefault(clientSocket) ?? string.Empty;

                // Add new information about the client to the server
                this.addressesAndUserNames.TryAdd(ipAddress, userName);
                this.userNamesAndSockets.TryAdd(userName, clientSocket);
                this.adminUsers.TryAdd(userName, false);
                this.mutedUsers.TryAdd(userName, false);

                while (this.isRunning)
                {
                    // Client force shutdown, if big red button pressed
                    if (clientSocket == null)
                    {
                        this.userNamesAndSockets.TryRemove(userName, out _);
                        this.addressesAndUserNames.TryRemove(ipAddress, out _);
                        break;
                    }

                    byte[] messageBuffer = new byte[Server.MESSAGE_MAXIMUM_SIZE];
                    int charactersReceivedCount = await clientSocket.ReceiveAsync(messageBuffer, SocketFlags.None);

                    // In case of a server timeout between waiting to receive a message
                    if (!this.isRunning)
                    {
                        break;
                    }

                    string messageContentReceived = Encoding.UTF8.GetString(messageBuffer, Server.STARTING_INDEX, charactersReceivedCount);

                    // Don't allow users to change info, the server does that
                    if (this.infoChangeCommandRegex.IsMatch(messageContentReceived))
                    {
                        continue;
                    }

                    // Check if the user disconnected
                    if (charactersReceivedCount == Server.DISCONNECT_CODE)
                    {
                        switch (this.IsHost(ipAddress))
                        {
                            case true:
                                messageContentReceived = "Host disconnected";
                                this.SendMessageToAllClients(CreateMessage(messageContentReceived, userName));
                                this.ShutDownServer();
                                break;
                            case false:
                                messageContentReceived = "Disconnected";
                                this.SendMessageToAllClients(CreateMessage(messageContentReceived, userName));
                                this.StartTimeoutIfMinimumConnectionsNotMet();
                                break;
                        }

                        this.RemoveClientInformation(clientSocket, userName, ipAddress);
                        break;
                    }

                    bool commandFound = true;

                    // Check if a command was received
                    switch (true)
                    {
                        case true when this.muteCommandRegex.IsMatch(messageContentReceived):
                            this.TryChangeStatus(messageContentReceived, Server.MUTE_STATUS, userName, clientSocket, this.mutedUsers);
                            break;
                        case true when this.adminCommandRegex.IsMatch(messageContentReceived):
                            this.TryChangeStatus(messageContentReceived, Server.ADMIN_STATUS, userName, clientSocket, this.adminUsers);
                            break;
                        case true when this.kickCommandRegex.IsMatch(messageContentReceived):
                            this.TryChangeStatus(messageContentReceived, Server.KICK_STATUS, userName, clientSocket);
                            break;
                        default:
                            commandFound = false;
                            break;
                    }

                    // Don't send the command to users
                    if (commandFound)
                    {
                        continue;
                    }

                    this.SendMessageToAllClients(CreateMessage(messageContentReceived, userName));
                }
            }
            catch (Exception)
            {
                // The function returns a Task, a task which is "disposed" (we can't wait for it to finish,
                // that would make the server only capable of handling one client), exceptions in this thread
                // don't affect the main thread anyway, so we just close the client connection
            }
            finally
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }

        private Message CreateMessage(string contentMessage, string userName)
        {
            // Creates a new message object (google protobuf auto-generated class)
            // Assign the time of the sent (from the server), senders name and status (beside the content)
            Message message = new Message
            {
                MessageContent = contentMessage,
                MessageDateTime = DateTime.Now.ToString(),
                MessageSenderName = userName,
                MessageAligment = "Left",
                MessageSenderStatus = this.GetHighestStatus(userName),
            };

            return message;
        }

        private void SendMessageToAllClients(Message message)
        {
            foreach (KeyValuePair<Socket, string> clientSocketsAndAddresses in this.socketsAndAddresses)
            {
                try
                {
                    this.SendMessageToOneClient(message, clientSocketsAndAddresses.Key);
                }
                catch (Exception)
                {
                    // The socket could be disposed
                }
            }
        }

        private void SendMessageToOneClient(Message message, Socket clientSocket)
        {
            byte[] messageBytes = message.ToByteArray();
            _ = clientSocket.SendAsync(messageBytes, SocketFlags.None);
        }

        private bool IsHost(string ipAddress)
        {
            return ipAddress == ipEndPoint.Address.ToString();
        }

        private void InitializeServerTimeout()
        {
            // Lock is used for thread safety
            lock (this.lockTimer)
            {
                // Dispose of a previous allocated timeout (if it exists)
                this.serverTimeout?.Dispose();
                // Recheck the condition after the countdown
                this.serverTimeout = new System.Threading.Timer((_) =>
                {
                    if (socketsAndAddresses.Count < Server.MINIMUM_CONNECTIONS)
                    {
                        this.ShutDownServer();
                    }
                }, null, Server.SERVER_TIMEOUT_COUNTDOWN, System.Threading.Timeout.Infinite);
            }
        }

        private void ShutDownServer()
        {
            // Check if it's connected, otherwise it throws an error
            if (this.serverSocket.Connected == true)
            {
                this.serverSocket.Shutdown(SocketShutdown.Both);
            }
            this.serverSocket.Close();
            this.isRunning = false;
        }

        private void StartTimeoutIfMinimumConnectionsNotMet()
        {
            if (this.socketsAndAddresses.Count < Server.MINIMUM_CONNECTIONS)
            {
                this.InitializeServerTimeout();
            }
        }

        public bool IsServerRunning()
        {
            return this.isRunning;
        }

        private string GetHighestStatus(string userName)
        {
            if (this.hostName == userName)
            {
                return Server.HOST_STATUS;
            }

            switch (this.adminUsers.GetValueOrDefault(userName))
            {
                case true:
                    return Server.ADMIN_STATUS;
                case false:
                    return Server.REGULAR_USER_STATUS;
            }
        }

        private bool IsUserAllowedOnTargetStatusChange(string firstUserStatus, string secondUserStatus)
        {
            // Host is not able to kick / mute / admin himself
            return (firstUserStatus == Server.HOST_STATUS && secondUserStatus != Server.HOST_STATUS)
                || (firstUserStatus == Server.ADMIN_STATUS && secondUserStatus == Server.REGULAR_USER_STATUS);
        }
        private string FindTargetedUserNameFromCommand(string command)
        {
            // Command follows the pattern : <username>|Status|<username>
            int commandTargetIndex = 2;
            char commandSeparator = '|';
            string commandTarget = command.Split(commandSeparator)[commandTargetIndex];

            int nameStartIndex = 1, nameEndIndex = commandTarget.Length - 2;
            string targetedUserName = commandTarget.Substring(nameStartIndex, nameEndIndex);

            return targetedUserName;
        }

        private void TryChangeStatus(string command, string targetedStatus, string userName, Socket userSocket, ConcurrentDictionary<string, bool>? statusDataHolder = null)
        {
            string targetedUserName = this.FindTargetedUserNameFromCommand(command);

            // Socket can be null if it's not found (the user never existed, if the command was written
            // or the user disconnected)
            Socket? targetedUserSocket = null;
            foreach (KeyValuePair<string, Socket> userNamesAndSockets in this.userNamesAndSockets)
            {
                if (targetedUserName == userNamesAndSockets.Key)
                {
                    targetedUserSocket = userNamesAndSockets.Value;
                    break;
                }
            }

            // If we don't find the user, cancel the operation
            if (targetedUserSocket == null)
            {
                return;
            }

            string targetedUserIpAddress = this.socketsAndAddresses.GetValueOrDefault(targetedUserSocket) ?? "Not found";

            string userStatus = this.GetHighestStatus(userName);
            string targetedUserStatus = this.GetHighestStatus(targetedUserName);

            if (this.IsUserAllowedOnTargetStatusChange(userStatus, targetedUserStatus))
            {
                // Add is ignored, since we already have his data, but if add happens, it takes the value false
                // Update operation should negate the current status that is targeted
                bool isStatus = statusDataHolder?.AddOrUpdate(targetedUserName, false, (key, oldValue) => !oldValue) ?? false;
                string messageContent;

                if (targetedStatus.Equals(Server.MUTE_STATUS))
                {
                    switch (isStatus)
                    {
                        case true:
                            messageContent = $"{targetedUserName} has been muted";
                            break;
                        case false:
                            messageContent = $"{targetedUserName} has been unmuted";
                            break;
                    }
                    this.SendMessageToOneClient(CreateMessage(Server.INFO_CHANGE_MUTE_STATUS_COMMAND, targetedUserName), targetedUserSocket);
                }
                else if (targetedStatus.Equals(Server.ADMIN_STATUS))
                {
                    switch (isStatus)
                    {
                        case true:
                            messageContent = $"{targetedUserName} is now an admin";
                            break;
                        case false:
                            messageContent = $"{targetedUserName} is no longer an admin";
                            break;
                    }
                    this.SendMessageToOneClient(CreateMessage(Server.INFO_CHANGE_ADMIN_STATUS_COMMAND, targetedUserName), targetedUserSocket);
                }
                else
                {
                    messageContent = $"{targetedUserName} has been kicked";
                    this.SendMessageToOneClient(CreateMessage(Server.INFO_CHANGE_KICK_STATUS_COMMAND, targetedUserName), targetedUserSocket);
                    this.RemoveClientInformation(targetedUserSocket, targetedUserName, targetedUserIpAddress);
                }
                this.SendMessageToAllClients(CreateMessage(messageContent, userName));
                return;
            }
            this.SendMessageToOneClient(CreateMessage(Server.SERVER_REJECT_COMMAND, this.hostName), userSocket);
        }

        private void RemoveClientInformation(Socket clientSocket, string userName, string ipAddress)
        {
            this.addressesAndUserNames.TryRemove(ipAddress, out _);
            this.socketsAndAddresses.TryRemove(clientSocket, out _);
            this.userNamesAndSockets.TryRemove(userName, out _);
            this.adminUsers.TryRemove(userName, out _);
            this.mutedUsers.TryRemove(userName, out _);
        }
        */
    }
}
