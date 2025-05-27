using System.Collections.Generic;
using System.Linq;
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
        private int friendId;

        private bool isAdmin;
        private bool isHost;
        private bool isMuted;
        private DispatcherTimer timer;
        public event EventHandler<bool> WindowClosed;

        public ObservableCollection<ChatMessageUI> Messages
        {
            get => this.messages;
        }

        private bool IsOpen { get; set; }
        public ChatRoomWindow(int myId_param, int friendId_param)
        {
            this.InitializeComponent();

            // Extra buttons: Admin/Mute/Kick
            this.HideExtraButtonsFromUser();

            this.myId = myId_param;
            this.friendId = friendId_param;
            this.messages = new ObservableCollection<ChatMessageUI>();
            this.service = new ChatService(App.ChatRepository);
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(300);
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private void Timer_Tick(object? sender, object e)
        {
            List<ChatMessage> messages = this.service.GetAllMessages(this.myId, friendId);
            foreach (ChatMessage msg in messages)
            {
                if (!this.messages.Any(m => m.MessageId == msg.MessageId))
                {
                    this.HandleNewMessage(msg);
                }
            }
        }

        public void Send_Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            ChatMessage msg = this.service.SendMessage(this.myId, this.friendId, this.MessageTextBox.Text);
            this.HandleNewMessage(msg);
            this.MessageTextBox.Text = string.Empty;
        }

        public void Mute_Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
          
        }

        public void Admin_Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            
        }

        public void Kick_Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {

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

        private void HandleNewMessage(ChatMessage msg)
        {
            User user = App.UserService.GetUserByIdentifier(msg.SenderId);
            DateTime time = DateTimeOffset.FromUnixTimeMilliseconds(msg.Timestamp).DateTime;
            ChatMessageUI messageUi = new ChatMessageUI()
            {
                MessageId = msg.MessageId,
                ConversationId = msg.ConversationId,
                MessageFormat = msg.MessageFormat,
                Timestamp = msg.Timestamp,
                SenderId = msg.SenderId,
                SenderUsername = user.Username,
                MessageContent = msg.MessageContent,
                Aligment = msg.SenderId == this.myId ? "Right" : "Left",
                Time = time.ToString("HH:mm | dd-MM-yyyy"),
            };
            this.messages.Add(messageUi);
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
