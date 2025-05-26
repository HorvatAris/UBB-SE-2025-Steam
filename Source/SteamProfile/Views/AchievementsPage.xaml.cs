using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using SteamProfile.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using BusinessLayer.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SteamProfile.Views
{
    public sealed partial class AchievementsPage : Page
    {
        private readonly AchievementsViewModel achievementsViewModel;
        public AchievementsPage()
        {
            this.InitializeComponent();
            achievementsViewModel = AchievementsViewModel.Instance;
            DataContext = achievementsViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await achievementsViewModel.LoadAchievementsAsync();
        }
    }
}
