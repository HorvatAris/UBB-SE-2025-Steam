using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SteamProfile.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel ViewModel { get; private set; }

        public LoginPage()
        {
            this.InitializeComponent();
            this.Loaded += LoginPage_Loaded;
        }

        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new LoginViewModel(this.Frame);
            this.DataContext = ViewModel;
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ForgotPasswordPage));
        }
    }
}
