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
using BusinessLayer.Services;
using SteamProfile.ViewModels;
using SteamProfile.Views.ConfigurationsView;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SteamHub.Pages
{
    public sealed partial class ConfigurationsPage : Page
    {
        public ConfigurationsViewModel ViewModel { get; private set; }
        private readonly UserService userService;
        public ConfigurationsPage(UserService userService)
        {
            this.InitializeComponent();
            this.Loaded += ConfigurationsPage_Loaded;
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        private void ConfigurationsPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new ConfigurationsViewModel(this.Frame, userService);
            this.DataContext = ViewModel;
        }
    }
}
