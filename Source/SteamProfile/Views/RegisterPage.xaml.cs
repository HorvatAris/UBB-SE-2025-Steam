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
using Windows.Foundation;
using Windows.Foundation.Collections;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class RegisterPage : Page
    {
        public RegisterViewModel ViewModel { get; private set; }

        public RegisterPage()
        {
            this.InitializeComponent();
            this.Loaded += RegisterPage_Loaded;
        }

        private void RegisterPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new RegisterViewModel(this.Frame);
            this.DataContext = ViewModel;
        }
    }
}
