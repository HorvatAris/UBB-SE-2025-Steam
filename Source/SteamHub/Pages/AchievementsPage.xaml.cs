using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SteamHub.ViewModels;
using SteamHub.ApiContract.Services.Interfaces;
using System.Threading.Tasks;
using System.Diagnostics;
using SteamHub.ApiContract.Models.User;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SteamHub.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AchievementsPage : Page
    {
        private readonly AchievementsViewModel achievementsViewModel;
        private bool _isInitialized = false;

        public AchievementsPage(IAchievementsService achievementService, IUserService userService)
        {
            Debug.WriteLine("AchievementsPage constructor called");
            this.InitializeComponent();
            achievementsViewModel = new AchievementsViewModel(achievementService, userService);
            DataContext = achievementsViewModel;

            // Subscribe to the Loaded event
            this.Loaded += AchievementsPage_Loaded;
        }

        private async void AchievementsPage_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("AchievementsPage_Loaded called");
            if (!_isInitialized)
            {
                System.Diagnostics.Debug.WriteLine("Attempting to load achievements");
                try
                {
                    await LoadAchievementsAsync();
                    _isInitialized = true;
                    System.Diagnostics.Debug.WriteLine("Achievements loaded successfully");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in AchievementsPage_Loaded: {ex}");
                    // Show error to user
                    var dialog = new ContentDialog
                    {
                        Title = "Error Loading Achievements",
                        Content = $"Failed to load achievements: {ex.Message}",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await dialog.ShowAsync();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Page already initialized, skipping load");
            }
        }

        private async Task LoadAchievementsAsync()
        {
            System.Diagnostics.Debug.WriteLine("LoadAchievementsAsync started");
            if (achievementsViewModel == null)
            {
                System.Diagnostics.Debug.WriteLine("AchievementsViewModel is null!");
                throw new InvalidOperationException("AchievementsViewModel is not initialized");
            }

            await achievementsViewModel.LoadAchievementsAsync();
            System.Diagnostics.Debug.WriteLine("LoadAchievementsAsync completed");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Debug.WriteLine("AchievementsPage OnNavigatedTo called");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Debug.WriteLine("AchievementsPage OnNavigatedFrom called");
        }
    }
}
