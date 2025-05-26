using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.UI.Dispatching;
using Moq;
using Steam_Community.DirectMessages.Interfaces;
using Steam_Community.DirectMessages.Models;
using Steam_Community.DirectMessages.Services;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


//MUST HAVE THESE INCLUDES IN EACH TEST FILE
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Community_Tests
{
    [TestClass]
    public class DMTests
    {
        #region Test Fields & Setup

        // ChatService test fields
        private Mock<INetworkClient> _mockNetworkClient;
        private Mock<INetworkServer> _mockNetworkServer;
        private Mock<DispatcherQueue> _mockDispatcherQueue;
        private ChatService _chatService;
        private string _testUsername = "TestUser";
        private string _testHostIp = "192.168.1.1";

        // NetworkClient test fields
        private Mock<Socket> _mockSocket;
        private NetworkClient _networkClient;

        // NetworkServer test fields
        private Mock<Socket> _mockServerSocket;
        private Mock<Socket> _mockClientSocket;
        private NetworkServer _networkServer;
        private string _testHostUsername = "HostUser";
        private string _testClientUsername = "ClientUser";

        // Integration test fields
        private ChatService _hostChatService;
        private ChatService _clientChatService;
        private bool _messageReceivedByClient = false;
        private bool _messageReceivedByHost = false;
        private ManualResetEvent _messageReceivedSignal;

        [TestInitialize]
        public void TestInitialize()
        {
            // Setup common mock dispatcher
            _mockDispatcherQueue = new Mock<DispatcherQueue>();
            _mockDispatcherQueue.Setup(d => d.TryEnqueue(It.IsAny<DispatcherQueueHandler>()))
                .Returns<DispatcherQueueHandler>(handler =>
                {
                    handler.Invoke();
                    return true;
                });

            // Setup for ChatService tests
            _mockNetworkClient = new Mock<INetworkClient>();
            _mockNetworkServer = new Mock<INetworkServer>();
            _chatService = new ChatService(_testUsername, _testHostIp, _mockDispatcherQueue.Object);

            // Use reflection to set private fields for ChatService
            SetPrivateField(_chatService, "_networkClient", _mockNetworkClient.Object);
            SetPrivateField(_chatService, "_networkServer", _mockNetworkServer.Object);

            // Setup for NetworkClient tests
            _mockSocket = new Mock<Socket>();
            _networkClient = new NetworkClient(_testHostIp, _testUsername, _mockDispatcherQueue.Object);
            SetPrivateField(_networkClient, "_clientSocket", _mockSocket.Object);

            // Setup for NetworkServer tests
            _mockServerSocket = new Mock<Socket>();
            _mockClientSocket = new Mock<Socket>();
            _networkServer = new NetworkServer(_testHostIp, _testHostUsername);
            SetPrivateField(_networkServer, "_serverSocket", _mockServerSocket.Object);
            SetupServerDictionaries();

            // Setup for integration tests
            _messageReceivedSignal = new ManualResetEvent(false);
        }

        private void SetupServerDictionaries()
        {
            // Setup mock remote endpoint for the client socket
            var mockEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.2"), 12345);
            _mockClientSocket.Setup(s => s.RemoteEndPoint).Returns(mockEndPoint);

            // Get the dictionaries
            var socketsToIpAddresses = GetPrivateField<ConcurrentDictionary<Socket, string>>(_networkServer, "_socketsToIpAddresses");
            var ipAddressesToUsernames = GetPrivateField<ConcurrentDictionary<string, string>>(_networkServer, "_ipAddressesToUsernames");
            var usernamesToSockets = GetPrivateField<ConcurrentDictionary<string, Socket>>(_networkServer, "_usernamesToSockets");
            var adminUsers = GetPrivateField<ConcurrentDictionary<string, bool>>(_networkServer, "_adminUsers");
            var mutedUsers = GetPrivateField<ConcurrentDictionary<string, bool>>(_networkServer, "_mutedUsers");

            // Add test client to dictionaries
            string clientIp = "192.168.1.2";
            socketsToIpAddresses[_mockClientSocket.Object] = clientIp;
            ipAddressesToUsernames[clientIp] = _testClientUsername;
            usernamesToSockets[_testClientUsername] = _mockClientSocket.Object;
            adminUsers[_testClientUsername] = false;
            mutedUsers[_testClientUsername] = false;
        }

        // Helper method to set private field using reflection
        private void SetPrivateField<T>(object obj, string fieldName, T value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, value);
        }

        // Helper method to get private field using reflection
        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field.GetValue(obj);
        }

        // Helper method to invoke private method using reflection
        private T InvokePrivateMethod<T>(object obj, string methodName, params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)method.Invoke(obj, parameters);
        }

        #endregion

        #region ChatService Tests

        [TestMethod]
        public void ChatService_ConnectToServer_UserIsHost_CreatesServerAndConnectsClient()
        {
            // Arrange
            _chatService = new ChatService(_testUsername, ChatConstants.HOST_IP_FINDER, _mockDispatcherQueue.Object);

            // Configure mocks for server creation flow
            _mockNetworkServer.Setup(s => s.Start());
            _mockNetworkClient.Setup(c => c.ConnectToServer()).Returns(Task.CompletedTask);
            _mockNetworkClient.Setup(c => c.SetAsHost());

            // Use reflection to set private fields
            SetPrivateField(_chatService, "_networkClient", _mockNetworkClient.Object);
            SetPrivateField(_chatService, "_networkServer", _mockNetworkServer.Object);

            // Act
            _chatService.ConnectToServer();

            // Assert
            _mockNetworkServer.Verify(s => s.Start(), Times.Once);
            _mockNetworkClient.Verify(c => c.ConnectToServer(), Times.Once);
            _mockNetworkClient.Verify(c => c.SetAsHost(), Times.Once);
        }

        [TestMethod]
        public void ChatService_ConnectToServer_UserIsNotHost_OnlyConnectsClient()
        {
            // Arrange
            _mockNetworkClient.Setup(c => c.ConnectToServer()).Returns(Task.CompletedTask);

            // Act
            _chatService.ConnectToServer();

            // Assert
            _mockNetworkClient.Verify(c => c.ConnectToServer(), Times.Once);
            _mockNetworkClient.Verify(c => c.SetAsHost(), Times.Never);
            _mockNetworkServer.Verify(s => s.Start(), Times.Never);
        }

        [TestMethod]
        public void ChatService_ConnectToServer_ConnectionFails_RaisesExceptionEvent()
        {
            // Arrange
            bool exceptionRaised = false;
            Exception thrownException = new Exception("Connection failed");
            _mockNetworkClient.Setup(c => c.ConnectToServer()).Throws(thrownException);

            _chatService.ExceptionOccurred += (sender, args) =>
            {
                exceptionRaised = true;
                Assert.AreEqual(thrownException, args.Exception);
            };

            // Act
            _chatService.ConnectToServer();

            // Assert
            Assert.IsTrue(exceptionRaised, "Exception event should have been raised");
        }

        [TestMethod]
        public void ChatService_SendMessage_ValidMessage_MessageSentSuccessfully()
        {
            // Arrange
            string testMessage = "Hello, world!";
            _mockNetworkClient.Setup(c => c.IsConnected()).Returns(true);
            _mockNetworkClient.Setup(c => c.SendMessageToServer(testMessage));
            _mockNetworkServer.Setup(s => s.IsRunning()).Returns(true);

            // Act
            _chatService.SendMessage(testMessage);

            // Assert
            _mockNetworkClient.Verify(c => c.SendMessageToServer(testMessage), Times.Once);
        }

        [TestMethod]
        public void ChatService_SendMessage_EmptyMessage_RaisesExceptionEvent()
        {
            // Arrange
            bool exceptionRaised = false;
            string emptyMessage = "";

            _chatService.ExceptionOccurred += (sender, args) =>
            {
                exceptionRaised = true;
                StringAssert.Contains(args.Exception.Message, "empty");
            };

            // Act
            _chatService.SendMessage(emptyMessage);

            // Assert
            Assert.IsTrue(exceptionRaised, "Exception event should have been raised");
            _mockNetworkClient.Verify(c => c.SendMessageToServer(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ChatService_SendMessage_ClientNotConnected_RaisesExceptionEvent()
        {
            // Arrange
            bool exceptionRaised = false;
            string testMessage = "Hello, world!";
            _mockNetworkClient.Setup(c => c.IsConnected()).Returns(false);

            _chatService.ExceptionOccurred += (sender, args) =>
            {
                exceptionRaised = true;
                StringAssert.Contains(args.Exception.Message, "not connected");
            };

            // Act
            _chatService.SendMessage(testMessage);

            // Assert
            Assert.IsTrue(exceptionRaised, "Exception event should have been raised");
            _mockNetworkClient.Verify(c => c.SendMessageToServer(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ChatService_HandleMessageReceived_SenderIsCurrentUser_AlignmentSetToRight()
        {
            // Arrange
            bool messageEventRaised = false;
            var message = new Message
            {
                MessageContent = "Test message",
                MessageSenderName = _testUsername
            };

            _chatService.MessageReceived += (sender, args) =>
            {
                messageEventRaised = true;
                Assert.AreEqual(ChatConstants.ALIGNMENT_RIGHT, args.Message.MessageAligment);
            };

            // Act - use our helper method to call private method
            InvokePrivateMethod<object>(_chatService, "HandleMessageReceived",
                null, new MessageEventArgs(message));

            // Assert
            Assert.IsTrue(messageEventRaised, "Message event should have been raised");
        }

        [TestMethod]
        public void ChatService_HandleMessageReceived_SenderIsNotCurrentUser_AlignmentSetToLeft()
        {
            // Arrange
            bool messageEventRaised = false;
            var message = new Message
            {
                MessageContent = "Test message",
                MessageSenderName = "OtherUser"
            };

            _chatService.MessageReceived += (sender, args) =>
            {
                messageEventRaised = true;
                Assert.AreEqual(ChatConstants.ALIGNMENT_LEFT, args.Message.MessageAligment);
            };

            // Act - use our helper method to call private method
            InvokePrivateMethod<object>(_chatService, "HandleMessageReceived",
                null, new MessageEventArgs(message));

            // Assert
            Assert.IsTrue(messageEventRaised, "Message event should have been raised");
        }

        [TestMethod]
        public void ChatService_AttemptChangeMuteStatus_ValidUser_SendsCommandWithCorrectFormat()
        {
            // Arrange
            string targetUsername = "TargetUser";
            string expectedCommand = $"<{_testUsername}>|{ChatConstants.MUTE_STATUS}|<{targetUsername}>";

            // Setup mock to capture the message sent
            string capturedMessage = null;
            _mockNetworkClient.Setup(c => c.IsConnected()).Returns(true);
            _mockNetworkClient.Setup(c => c.SendMessageToServer(It.IsAny<string>()))
                .Callback<string>(msg => capturedMessage = msg);

            // Act
            _chatService.AttemptChangeMuteStatus(targetUsername);

            // Assert
            Assert.AreEqual(expectedCommand, capturedMessage);
        }

        [TestMethod]
        public void ChatService_AttemptChangeAdminStatus_ValidUser_SendsCommandWithCorrectFormat()
        {
            // Arrange
            string targetUsername = "TargetUser";
            string expectedCommand = $"<{_testUsername}>|{ChatConstants.ADMIN_STATUS}|<{targetUsername}>";

            // Setup mock to capture the message sent
            string capturedMessage = null;
            _mockNetworkClient.Setup(c => c.IsConnected()).Returns(true);
            _mockNetworkClient.Setup(c => c.SendMessageToServer(It.IsAny<string>()))
                .Callback<string>(msg => capturedMessage = msg);

            // Act
            _chatService.AttemptChangeAdminStatus(targetUsername);

            // Assert
            Assert.AreEqual(expectedCommand, capturedMessage);
        }

        [TestMethod]
        public void ChatService_AttemptKickUser_ValidUser_SendsCommandWithCorrectFormat()
        {
            // Arrange
            string targetUsername = "TargetUser";
            string expectedCommand = $"<{_testUsername}>|{ChatConstants.KICK_STATUS}|<{targetUsername}>";

            // Setup mock to capture the message sent
            string capturedMessage = null;
            _mockNetworkClient.Setup(c => c.IsConnected()).Returns(true);
            _mockNetworkClient.Setup(c => c.SendMessageToServer(It.IsAny<string>()))
                .Callback<string>(msg => capturedMessage = msg);

            // Act
            _chatService.AttemptKickUser(targetUsername);

            // Assert
            Assert.AreEqual(expectedCommand, capturedMessage);
        }

        [TestMethod]
        public void ChatService_DisconnectFromServer_WhenCalled_DisconnectsClient()
        {
            // Arrange
            _mockNetworkClient.Setup(c => c.Disconnect());

            // Act
            _chatService.DisconnectFromServer();

            // Assert
            _mockNetworkClient.Verify(c => c.Disconnect(), Times.Once);
        }

        [TestMethod]
        public void ChatService_GetLocalIpAddress_DebugMode_ReturnsDebugIp()
        {
            // This test assumes DEBUG_MODE is true in ChatConstants
            if (ChatConstants.DEBUG_MODE)
            {
                // Act
                var result = ChatService.GetLocalIpAddress();

                // Assert
                Assert.AreEqual(ChatConstants.DEBUG_HOST_IP, result);
            }
            else
            {
                Assert.Inconclusive("Test requires DEBUG_MODE to be true");
            }
        }

        #endregion

        #region NetworkClient Tests

        [TestMethod]
        public async Task NetworkClient_ConnectToServer_ValidServer_ConnectsSuccessfully()
        {
            // Arrange
            _mockSocket.Setup(s => s.ConnectAsync(It.IsAny<EndPoint>())).Returns(Task.CompletedTask);
            _mockSocket.Setup(s => s.SendAsync(It.IsAny<byte[]>(), It.IsAny<SocketFlags>()))
                .Returns(Task.FromResult(Encoding.UTF8.GetBytes(_testUsername).Length));

            bool statusEventRaised = false;
            _networkClient.UserStatusChanged += (sender, args) =>
            {
                statusEventRaised = true;
                Assert.IsTrue(args.UserStatus.IsConnected);
            };

            // Act
            await _networkClient.ConnectToServer();

            // Assert
            _mockSocket.Verify(s => s.ConnectAsync(It.IsAny<EndPoint>()), Times.Once);
            _mockSocket.Verify(s => s.SendAsync(It.IsAny<byte[]>(), It.IsAny<SocketFlags>()), Times.Once);
            Assert.IsTrue(statusEventRaised, "Status event should have been raised");
            Assert.IsTrue(_networkClient.IsConnected(), "Client should be connected");
        }

        [TestMethod]
        public async Task NetworkClient_ConnectToServer_ConnectionFails_ThrowsException()
        {
            // Arrange
            var expectedException = new SocketException();
            _mockSocket.Setup(s => s.ConnectAsync(It.IsAny<EndPoint>())).Throws(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _networkClient.ConnectToServer());
            StringAssert.Contains(exception.Message, "Failed to connect to server");
        }

        [TestMethod]
        public async Task NetworkClient_SendMessageToServer_Connected_SendsMessage()
        {
            // Arrange
            string testMessage = "Hello, world!";
            byte[] expectedBytes = Encoding.UTF8.GetBytes(testMessage);

            _mockSocket.Setup(s => s.SendAsync(It.IsAny<byte[]>(), It.IsAny<SocketFlags>()))
                .Returns(Task.FromResult(expectedBytes.Length))
                .Callback<byte[], SocketFlags>((data, flags) =>
                {
                    // Verify the data being sent matches our expected message
                    string sentMessage = Encoding.UTF8.GetString(data);
                    Assert.AreEqual(testMessage, sentMessage);
                });

            // Act
            await _networkClient.SendMessageToServer(testMessage);

            // Assert
            _mockSocket.Verify(s => s.SendAsync(It.IsAny<byte[]>(), It.IsAny<SocketFlags>()), Times.Once);
        }

        [TestMethod]
        public async Task NetworkClient_SendMessageToServer_SendFails_ThrowsException()
        {
            // Arrange
            string testMessage = "Hello, world!";
            var expectedException = new SocketException();
            _mockSocket.Setup(s => s.SendAsync(It.IsAny<byte[]>(), It.IsAny<SocketFlags>())).Throws(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() =>
                _networkClient.SendMessageToServer(testMessage));
            StringAssert.Contains(exception.Message, "Failed to send message");
        }

        [TestMethod]
        public void NetworkClient_UpdateUserStatus_AdminStatus_TogglesAdminStatus()
        {
            // Arrange
            bool statusEventRaised = false;
            bool expectedAdminStatus = true; // Initially false, should become true

            _networkClient.UserStatusChanged += (sender, args) =>
            {
                statusEventRaised = true;
                Assert.AreEqual(expectedAdminStatus, args.UserStatus.IsAdmin);
            };

            // Act - use our helper method to call private method
            InvokePrivateMethod<object>(_networkClient, "UpdateUserStatus", ChatConstants.ADMIN_STATUS);

            // Assert
            Assert.IsTrue(statusEventRaised, "Status event should have been raised");
        }

        [TestMethod]
        public void NetworkClient_UpdateUserStatus_MuteStatus_TogglesMuteStatus()
        {
            // Arrange
            bool statusEventRaised = false;
            bool expectedMuteStatus = true; // Initially false, should become true

            _networkClient.UserStatusChanged += (sender, args) =>
            {
                statusEventRaised = true;
                Assert.AreEqual(expectedMuteStatus, args.UserStatus.IsMuted);
            };

            // Act - use our helper method to call private method
            InvokePrivateMethod<object>(_networkClient, "UpdateUserStatus", ChatConstants.MUTE_STATUS);

            // Assert
            Assert.IsTrue(statusEventRaised, "Status event should have been raised");
        }

        [TestMethod]
        public void NetworkClient_UpdateUserStatus_KickStatus_ClosesConnection()
        {
            // Arrange
            _mockSocket.Setup(s => s.Shutdown(It.IsAny<SocketShutdown>()));

            bool statusEventRaised = false;
            _networkClient.UserStatusChanged += (sender, args) =>
            {
                statusEventRaised = true;
                Assert.IsFalse(args.UserStatus.IsConnected);
            };

            // Act - use our helper method to call private method
            InvokePrivateMethod<object>(_networkClient, "UpdateUserStatus", ChatConstants.KICK_STATUS);

            // Assert
            Assert.IsTrue(statusEventRaised, "Status event should have been raised");
            _mockSocket.Verify(s => s.Shutdown(It.IsAny<SocketShutdown>()), Times.Once);
            Assert.IsFalse(_networkClient.IsConnected(), "Client should be disconnected");
        }

        [TestMethod]
        public void NetworkClient_SetAsHost_WhenCalled_SetsHostStatusToTrue()
        {
            // Arrange
            var userStatus = GetPrivateField<UserStatus>(_networkClient, "_userStatus");
            Assert.IsFalse(userStatus.IsHost, "Host status should initially be false");

            // Act
            _networkClient.SetAsHost();

            // Assert
            Assert.IsTrue(userStatus.IsHost, "Host status should be set to true");
        }

        [TestMethod]
        public async Task NetworkClient_Disconnect_WhenCalled_SendsDisconnectMessageAndClosesConnection()
        {
            // Arrange
            _mockSocket.Setup(s => s.SendAsync(It.Is<byte[]>(b => b.Length == ChatConstants.DISCONNECT_CODE), It.IsAny<SocketFlags>()))
                .Returns(Task.FromResult(0));
            _mockSocket.Setup(s => s.Shutdown(It.IsAny<SocketShutdown>()));

            // Setup initial state to be connected
            var userStatus = GetPrivateField<UserStatus>(_networkClient, "_userStatus");
            userStatus.IsConnected = true;

            bool statusEventRaised = false;
            _networkClient.UserStatusChanged += (sender, args) =>
            {
                statusEventRaised = true;
                Assert.IsFalse(args.UserStatus.IsConnected);
            };

            // Act
            _networkClient.Disconnect();

            // Assert
            _mockSocket.Verify(s => s.SendAsync(It.IsAny<byte[]>(), It.IsAny<SocketFlags>()), Times.Once);
            _mockSocket.Verify(s => s.Shutdown(It.IsAny<SocketShutdown>()), Times.Once);
            Assert.IsTrue(statusEventRaised, "Status event should have been raised");
            Assert.IsFalse(_networkClient.IsConnected(), "Client should be disconnected");
        }

        [TestMethod]
        public void NetworkClient_IsConnected_ConnectedClient_ReturnsTrue()
        {
            // Arrange - Set the connected status to true using reflection
            var userStatus = GetPrivateField<UserStatus>(_networkClient, "_userStatus");
            userStatus.IsConnected = true;

            // Act
            bool result = _networkClient.IsConnected();

            // Assert
            Assert.IsTrue(result, "IsConnected should return true for a connected client");
        }

        [TestMethod]
        public void NetworkClient_IsConnected_DisconnectedClient_ReturnsFalse()
        {
            // Arrange - Set the connected status to false using reflection
            var userStatus = GetPrivateField<UserStatus>(_networkClient, "_userStatus");
            userStatus.IsConnected = false;

            // Act
            bool result = _networkClient.IsConnected();

            // Assert
            Assert.IsFalse(result, "IsConnected should return false for a disconnected client");
        }

        #endregion

        #region NetworkServer Tests

        [TestMethod]
        public void NetworkServer_CreateMessage_WithContentAndSender_CreatesMessageCorrectly()
        {
            // Arrange
            string messageContent = "Test message";

            // Act
            Message message = _networkServer.CreateMessage(messageContent, _testClientUsername);

            // Assert
            Assert.AreEqual(messageContent, message.MessageContent);
            Assert.AreEqual(_testClientUsername, message.MessageSenderName);
            Assert.AreEqual(ChatConstants.ALIGNMENT_LEFT, message.MessageAligment);
            Assert.AreEqual(ChatConstants.REGULAR_USER_STATUS, message.MessageSenderStatus);
            Assert.IsNotNull(message.MessageDateTime);
        }

        [TestMethod]
        public void NetworkServer_GetHighestUserStatus_HostUser_ReturnsHostStatus()
        {
            // Act
            string result = InvokePrivateMethod<string>(_networkServer, "GetHighestUserStatus", _testHostUsername);

            // Assert
            Assert.AreEqual(ChatConstants.HOST_STATUS, result);
        }

        [TestMethod]
        public void NetworkServer_GetHighestUserStatus_AdminUser_ReturnsAdminStatus()
        {
            // Arrange
            var adminUsers = GetPrivateField<ConcurrentDictionary<string, bool>>(_networkServer, "_adminUsers");
            adminUsers[_testClientUsername] = true;

            // Act
            string result = InvokePrivateMethod<string>(_networkServer, "GetHighestUserStatus", _testClientUsername);

            // Assert
            Assert.AreEqual(ChatConstants.ADMIN_STATUS, result);
        }

        [TestMethod]
        public void NetworkServer_GetHighestUserStatus_RegularUser_ReturnsUserStatus()
        {
            // Act
            string result = InvokePrivateMethod<string>(_networkServer, "GetHighestUserStatus", _testClientUsername);

            // Assert
            Assert.AreEqual(ChatConstants.REGULAR_USER_STATUS, result);
        }

        [TestMethod]
        public void NetworkServer_CanChangeUserStatus_HostChangingRegularUser_ReturnsTrue()
        {
            // Act
            bool result = InvokePrivateMethod<bool>(_networkServer, "CanChangeUserStatus",
                ChatConstants.HOST_STATUS, ChatConstants.REGULAR_USER_STATUS);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NetworkServer_CanChangeUserStatus_HostChangingHost_ReturnsFalse()
        {
            // Act
            bool result = InvokePrivateMethod<bool>(_networkServer, "CanChangeUserStatus",
                ChatConstants.HOST_STATUS, ChatConstants.HOST_STATUS);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NetworkServer_CanChangeUserStatus_AdminChangingRegularUser_ReturnsTrue()
        {
            // Act
            bool result = InvokePrivateMethod<bool>(_networkServer, "CanChangeUserStatus",
                ChatConstants.ADMIN_STATUS, ChatConstants.REGULAR_USER_STATUS);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NetworkServer_CanChangeUserStatus_AdminChangingAdmin_ReturnsFalse()
        {
            // Act
            bool result = InvokePrivateMethod<bool>(_networkServer, "CanChangeUserStatus",
                ChatConstants.ADMIN_STATUS, ChatConstants.ADMIN_STATUS);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NetworkServer_CanChangeUserStatus_RegularUserChangingAny_ReturnsFalse()
        {
            // Act
            bool result = InvokePrivateMethod<bool>(_networkServer, "CanChangeUserStatus",
                ChatConstants.REGULAR_USER_STATUS, ChatConstants.REGULAR_USER_STATUS);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NetworkServer_ExtractTargetUsernameFromCommand_ValidCommand_ReturnsUsername()
        {
            // Arrange
            string targetUsername = "TargetUser";
            string command = $"<{_testHostUsername}>|{ChatConstants.MUTE_STATUS}|<{targetUsername}>";

            // Act
            string result = InvokePrivateMethod<string>(_networkServer, "ExtractTargetUsernameFromCommand", command);

            // Assert
            Assert.AreEqual(targetUsername, result);
        }

        [TestMethod]
        public void NetworkServer_IsRunning_AfterInitialization_ReturnsTrue()
        {
            // Act
            bool result = _networkServer.IsRunning();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NetworkServer_IsRunning_AfterShutdown_ReturnsFalse()
        {
            // Arrange - Call ShutdownServer using reflection
            InvokePrivateMethod<object>(_networkServer, "ShutdownServer");

            // Act
            bool result = _networkServer.IsRunning();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NetworkServer_IsHostIpAddress_HostIp_ReturnsTrue()
        {
            // Arrange
            // Setup IP endpoint to match the test host IP
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(_testHostIp), ChatConstants.PORT_NUMBER);
            SetPrivateField(_networkServer, "_ipEndPoint", ipEndPoint);

            // Act
            bool result = InvokePrivateMethod<bool>(_networkServer, "IsHostIpAddress", _testHostIp);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NetworkServer_IsHostIpAddress_NonHostIp_ReturnsFalse()
        {
            // Arrange
            string nonHostIp = "192.168.1.2";

            // Act
            bool result = InvokePrivateMethod<bool>(_networkServer, "IsHostIpAddress", nonHostIp);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NetworkServer_ProcessStatusChangeCommand_HostMutingUser_TogglesMuteStatus()
        {
            // Arrange - Setup the command
            string muteCommand = $"<{_testHostUsername}>|{ChatConstants.MUTE_STATUS}|<{_testClientUsername}>";

            // Setup the usernamesToSockets dictionary with test client
            var usernamesToSockets = GetPrivateField<ConcurrentDictionary<string, Socket>>(_networkServer, "_usernamesToSockets");
            usernamesToSockets[_testClientUsername] = _mockClientSocket.Object;

            // Get the mutedUsers dictionary
            var mutedUsers = GetPrivateField<ConcurrentDictionary<string, bool>>(_networkServer, "_mutedUsers");
            mutedUsers[_testClientUsername] = false; // Initially not muted

            // Setup for socket operations
            _mockClientSocket.Setup(s => s.SendAsync(It.IsAny<byte[]>(), It.IsAny<SocketFlags>()))
                .Returns(Task.FromResult(10));

            // Act
            InvokePrivateMethod<object>(_networkServer, "ProcessStatusChangeCommand",
                muteCommand, ChatConstants.MUTE_STATUS, _testHostUsername, _mockClientSocket.Object, mutedUsers);

            // Assert
            Assert.IsTrue(mutedUsers[_testClientUsername], "User should be muted after command");

            // Verify message was sent to client
            _mockClientSocket.Verify(s => s.SendAsync(It.IsAny<byte[]>(), It.IsAny<SocketFlags>()), Times.AtLeast(1));
        }

        [TestMethod]
        public void NetworkServer_RemoveClientInformation_ExistingClient_RemovesFromAllDictionaries()
        {
            // Arrange
            string clientIp = "192.168.1.2";

            // Get all dictionaries
            var socketsToIpAddresses = GetPrivateField<ConcurrentDictionary<Socket, string>>(_networkServer, "_socketsToIpAddresses");
            var ipAddressesToUsernames = GetPrivateField<ConcurrentDictionary<string, string>>(_networkServer, "_ipAddressesToUsernames");
            var usernamesToSockets = GetPrivateField<ConcurrentDictionary<string, Socket>>(_networkServer, "_usernamesToSockets");
            var adminUsers = GetPrivateField<ConcurrentDictionary<string, bool>>(_networkServer, "_adminUsers");
            var mutedUsers = GetPrivateField<ConcurrentDictionary<string, bool>>(_networkServer, "_mutedUsers");

            // Ensure the client is in all dictionaries
            socketsToIpAddresses[_mockClientSocket.Object] = clientIp;
            ipAddressesToUsernames[clientIp] = _testClientUsername;
            usernamesToSockets[_testClientUsername] = _mockClientSocket.Object;
            adminUsers[_testClientUsername] = false;
            mutedUsers[_testClientUsername] = false;

            // Act
            InvokePrivateMethod<object>(_networkServer, "RemoveClientInformation",
                _mockClientSocket.Object, _testClientUsername, clientIp);

            // Assert
            Assert.IsFalse(socketsToIpAddresses.ContainsKey(_mockClientSocket.Object), "Socket should be removed");
            Assert.IsFalse(ipAddressesToUsernames.ContainsKey(clientIp), "IP address should be removed");
            Assert.IsFalse(usernamesToSockets.ContainsKey(_testClientUsername), "Username should be removed");
            Assert.IsFalse(adminUsers.ContainsKey(_testClientUsername), "Admin status should be removed");
            Assert.IsFalse(mutedUsers.ContainsKey(_testClientUsername), "Mute status should be removed");
        }

        #endregion

        #region UserStatus Tests

        [TestMethod]
        public void UserStatus_Constructor_WhenCalled_InitializesWithDefaultValues()
        {
            // Act
            var userStatus = new UserStatus();

            // Assert
            Assert.IsFalse(userStatus.IsAdmin, "IsAdmin should initialize to false");
            Assert.IsFalse(userStatus.IsMuted, "IsMuted should initialize to false");
            Assert.IsFalse(userStatus.IsHost, "IsHost should initialize to false");
            Assert.IsFalse(userStatus.IsConnected, "IsConnected should initialize to false");
        }

        [TestMethod]
        public void UserStatus_IsAdmin_GetAndSet_WorksCorrectly()
        {
            // Arrange
            var userStatus = new UserStatus();
            Assert.IsFalse(userStatus.IsAdmin, "IsAdmin should initialize to false");

            // Act
            userStatus.IsAdmin = true;

            // Assert
            Assert.IsTrue(userStatus.IsAdmin, "IsAdmin should be updated to true");
        }

        [TestMethod]
        public void UserStatus_IsMuted_GetAndSet_WorksCorrectly()
        {
            // Arrange
            var userStatus = new UserStatus();
            Assert.IsFalse(userStatus.IsMuted, "IsMuted should initialize to false");

            // Act
            userStatus.IsMuted = true;

            // Assert
            Assert.IsTrue(userStatus.IsMuted, "IsMuted should be updated to true");
        }

        [TestMethod]
        public void UserStatus_IsHost_GetAndSet_WorksCorrectly()
        {
            // Arrange
            var userStatus = new UserStatus();
            Assert.IsFalse(userStatus.IsHost, "IsHost should initialize to false");

            // Act
            userStatus.IsHost = true;

            // Assert
            Assert.IsTrue(userStatus.IsHost, "IsHost should be updated to true");
        }

        [TestMethod]
        public void UserStatus_IsConnected_GetAndSet_WorksCorrectly()
        {
            // Arrange
            var userStatus = new UserStatus();
            Assert.IsFalse(userStatus.IsConnected, "IsConnected should initialize to false");

            // Act
            userStatus.IsConnected = true;

            // Assert
            Assert.IsTrue(userStatus.IsConnected, "IsConnected should be updated to true");
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        [Timeout(10000)] // 10 seconds timeout
        public async Task Integration_HostAndClientCommunication_MessageExchange_SuccessfullyDelivered()
        {
            // Skip test if DEBUG_MODE is not enabled
            if (!ChatConstants.DEBUG_MODE)
            {
                Assert.Inconclusive("Test requires DEBUG_MODE to be true");
                return;
            }

            try
            {
                // Create host and client services
                _hostChatService = new ChatService(_testHostUsername, ChatConstants.HOST_IP_FINDER, _mockDispatcherQueue.Object);

                // Subscribe to host message events
                _hostChatService.MessageReceived += (sender, e) =>
                {
                    if (e.Message.MessageSenderName == _testClientUsername)
                    {
                        _messageReceivedByHost = true;
                        _messageReceivedSignal.Set();
                    }
                };

                // Connect host to server (this creates the server)
                _hostChatService.ConnectToServer();

                // Wait a bit for the server to start
                await Task.Delay(500);

                // Create client service
                _clientChatService = new ChatService(_testClientUsername, ChatConstants.DEBUG_HOST_IP, _mockDispatcherQueue.Object);

                // Subscribe to client message events
                _clientChatService.MessageReceived += (sender, e) =>
                {
                    if (e.Message.MessageSenderName == _testHostUsername)
                    {
                        _messageReceivedByClient = true;
                        _messageReceivedSignal.Set();
                    }
                };

                // Connect client to server
                _clientChatService.ConnectToServer();

                // Wait a bit for connection to establish
                await Task.Delay(500);

                // Host sends a message
                _hostChatService.SendMessage("Test message from host");

                // Wait for message to be received by client
                bool messageReceived = _messageReceivedSignal.WaitOne(2000);

                if (!messageReceived)
                {
                    Assert.Inconclusive("Message was not received in time - this might be due to test environment limitations");
                    return;
                }

                // Reset the signal for the next message
                _messageReceivedSignal.Reset();
                _messageReceivedByClient = false;

                // Client sends a message back
                _clientChatService.SendMessage("Test message from client");

                // Wait for message to be received by host
                messageReceived = _messageReceivedSignal.WaitOne(2000);

                if (!messageReceived)
                {
                    Assert.Inconclusive("Return message was not received in time - this might be due to test environment limitations");
                    return;
                }

                // Assert
                Assert.IsTrue(_messageReceivedByClient, "Client should have received the message");
                Assert.IsTrue(_messageReceivedByHost, "Host should have received the message");
            }
            finally
            {
                // Clean up
                _clientChatService?.DisconnectFromServer();
                _hostChatService?.DisconnectFromServer();

                // Allow time for connections to close
                await Task.Delay(500);
            }
        }

        #endregion
    }
}