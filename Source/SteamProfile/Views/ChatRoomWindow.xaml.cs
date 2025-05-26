using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel;
using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Services;

namespace SteamProfile.Views
{
    public partial class ChatRoomWindow : Window
    {
        private IChatService service;
        private ObservableCollection<ChatMessageUI> messages;

        private int myId;

        private bool isAdmin;
        private bool isHost;
        private bool isMuted;

        public event EventHandler<bool> WindowClosed;

        /// <summary>
        /// This property is bound to the ListView from the View
        /// </summary>
        public ObservableCollection<ChatMessageUI> Messages
        {
            get => this.messages;
        }

        /// <summary>
        /// This property is used to trigger a change in the text shown by the friend
        /// request button
        /// </summary>
        private bool IsOpen { get; set; }

        /*
        /// <summary>
        /// Creates a new window representing a chat room for users
        /// </summary>
        /// <param name="userName">The name of the user who joined the chat room</param>
        /// <param name="serverInviteIp">The ip of the person who invited the user
        ///                              Don't provide the argument if you want to host</param>
        public ChatRoomWindow(string userName, string serverInviteIp = Service.HOST_IP_FINDER)
        {
            this.InitializeComponent();

            // Extra buttons: Admin/Mute/Kick
            this.HideExtraButtonsFromUser();

            // In the client we use the thread pool, but we need to update the ui in the main thread, so we capture it
            Microsoft.UI.Dispatching.DispatcherQueue uiThread = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            this.userName = userName;
            this.IsOpen = true;
            this.messages = new ObservableCollection<Message>();
            this.service = new Service(userName, serverInviteIp, uiThread);

            // Events -> if something happened, alert the listeners, in this case we are the listeners
            //          and we assign functions for each trigger of an event
            this.service.NewMessageEvent += HandleNewMessage;
            this.service.ClientStatusChangedEvent += HandleUserStatusChange;
            this.service.ExceptionEvent += HandleException;

            this.Closed += this.DisconnectService;

            WaitAndConnectToTheServer();
        }
        */

        public ChatRoomWindow(int myId_param, int friendId)
        {
            this.InitializeComponent();

            // Extra buttons: Admin/Mute/Kick
            this.HideExtraButtonsFromUser();

            this.myId = myId_param;
            this.messages = new ObservableCollection<ChatMessageUI>();
            this.service = new ChatService(App.ChatRepository, this.myId, friendId);

            // Events -> if something happened, alert the listeners, in this case we are the listeners
            //          and we assign functions for each trigger of an event
            this.service.NewMessageEvent += HandleNewMessage;
            this.service.ExceptionEvent += HandleException;

            this.Closed += this.DisconnectService;

            WaitAndConnectToTheServer();
        }

        public void Send_Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            this.service.SendMessage(this.MessageTextBox.Text);
            // Clear the input
            this.MessageTextBox.Text = string.Empty;
        }

        public void Mute_Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            /*
            if (this.InvertedListView.SelectedItem is Message selectedMessage)
            {
                this.service.TryChangeMuteStatus(selectedMessage.MessageSenderName);
            }
            */
        }

        public void Admin_Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            /*
            if (this.InvertedListView.SelectedItem is Message selectedMessage)
            {
                this.service.TryChangeAdminStatus(selectedMessage.MessageSenderName);
            }
            */
        }

        public void Kick_Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            /*
            if (this.InvertedListView.SelectedItem is Message selectedMessage)
            {
                this.service.TryKick(selectedMessage.MessageSenderName);
            }
            */
        }

        public void Clear_Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            this.messages.Clear();
        }

        public void OnHighlightedMessageChange(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.InvertedListView.SelectedItem is ChatMessage message)
            {
                // Check if the current user sent the message, in which case hide these buttons
                switch (message.SenderId == this.myId)
                {
                    case true:
                        this.HideExtraButtonsFromUser();
                        break;
                    case false:
                        this.ShowAvailableButtons();
                        break;
                }
            }
        }

        private void HandleNewMessage(object? sender, MessageEventArgs messageEventArgs)
        {
            User user = App.UserService.GetUserByIdentifier(messageEventArgs.Message.SenderId);
            DateTime time = DateTimeOffset.FromUnixTimeMilliseconds(messageEventArgs.Message.Timestamp).DateTime;
            ChatMessageUI messageUi = new ChatMessageUI()
            {
                MessageId = messageEventArgs.Message.MessageId,
                ConversationId = messageEventArgs.Message.ConversationId,
                MessageFormat = messageEventArgs.Message.MessageFormat,
                Timestamp = messageEventArgs.Message.Timestamp,
                SenderId = messageEventArgs.Message.SenderId,
                SenderUsername = user.Username,
                MessageContent = messageEventArgs.Message.MessageContent,
                Aligment = messageEventArgs.Message.SenderId == this.myId ? "Right" : "Left",
                Time = time.ToString("HH:mm | dd-MM-yyyy"),
            };
            this.messages.Add(messageUi);

            // If the user has more than 100 message, we delete the oldest message, like specified in the
            // requirements of the dms
            while (this.messages.Count > 100)
            {
                this.messages.RemoveAt(0);
            }
        }

        private async void WaitAndConnectToTheServer()
        {
            // "XamlRoot" is required to display the errors
            while (this.Content.XamlRoot == null)
            {
                await Task.Delay(50);
            }
            this.service.ConnectUserToServer();
        }

        public void DisconnectService(object sender, WindowEventArgs args)
        {
            this.IsOpen = false;

            // Further call on the service (we attempt at disconnecting the client on window close)
            this.service.DisconnectClient();

            // Alert listeners about window closure
            if (this.WindowClosed != null)
            {
                this.WindowClosed.Invoke(this, true);
            }
        }
        private async void HandleException(object? sender, ExceptionEventArgs exceptionEventArgs)
        {
            // If somebody created this class, they could get an error if the window was closed fast
            // since the socket will attempt to connect for around 15 - 30 seconds
            if (!this.IsOpen)
            {
                return;
            }

            // ContentDialog is a pop up that tells about what exactly happened (the error message)
            ContentDialog errorDialog = new ContentDialog()
            {
                Title = "Request rejected!",
                Content = exceptionEventArgs.Exception.Message,
                CloseButtonText = "Ok",
                XamlRoot = this.Content.XamlRoot,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(230, 219, 112, 147)),
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                CornerRadius = new CornerRadius(8)
            };

            // AI generated style for the pop up (it fits with the background)
            errorDialog.Resources["ContentDialogButtonBackground"] =
                new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(255, 219, 112, 147));

            errorDialog.Resources["ContentDialogButtonForeground"] =
                new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.White);

            await errorDialog.ShowAsync();
        }

        private void HideExtraButtonsFromUser()
        {
            this.AdminButton.Visibility = Visibility.Collapsed;
            this.MuteButton.Visibility = Visibility.Collapsed;
            this.KickButton.Visibility = Visibility.Collapsed;
        }

        private void ShowAdminButtons()
        {
            this.MuteButton.Visibility = Visibility.Visible;
            this.KickButton.Visibility = Visibility.Visible;
        }

        private void ShowHostButtons()
        {
            this.AdminButton.Visibility = Visibility.Visible;
            this.ShowAdminButtons();
        }

        private void ShowAvailableButtons()
        {
            if (this.isHost)
            {
                this.ShowHostButtons();
            }
            else if (this.isAdmin)
            {
                this.ShowAdminButtons();
            }
            else
            {
                this.HideExtraButtonsFromUser();
            }

            // On mute, don't allow the user to send a message (hide the button)
            switch (this.isMuted)
            {
                case true:
                    this.SendButton.Visibility = Visibility.Collapsed;
                    break;
                case false:
                    this.SendButton.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
