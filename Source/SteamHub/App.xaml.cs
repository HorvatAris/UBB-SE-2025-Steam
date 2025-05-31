using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SteamHub
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        public static Window? MainWindow { get; private set; }

        /*
         * TEMPORARY: This is a placeholder for getting the Services from APP,
         * until we think of a solution I will keep them here. Sorry if it's a bother
         * 
        */
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();
        public static IFriendsService FriendsService { get; private set; }
        public static ICollectionsRepository CollectionsRepository { get; private set; }
        public static IUserService UserService { get; private set; }
        public static IFeaturesService FeaturesService { get; private set; }
        public static IAchievementsService AchievementsService { get; private set; }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow = new SteamHub.MainWindow();
            MainWindow.Activate();

            // Get the AppWindow
            var windowHandle = WindowNative.GetWindowHandle(MainWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // Set window position to center of screen
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
            var centerX = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
            var centerY = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;
            appWindow.Move(new Windows.Graphics.PointInt32((int)centerX, (int)centerY));

            // Set window size
            appWindow.Resize(new Windows.Graphics.SizeInt32(1200, 800));

            // Set window title
            appWindow.Title = "SteamHub";

            // Set window icon
            try
            {
                var iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "Assets", "CustomSteamLogo.png");
                if (System.IO.File.Exists(iconPath))
                {
                    var hIcon = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 32, 32, LR_LOADFROMFILE);
                    if (hIcon != IntPtr.Zero)
                    {
                        SendMessage(windowHandle, WM_SETICON, (IntPtr)ICON_BIG, hIcon);
                        SendMessage(windowHandle, WM_SETICON, (IntPtr)ICON_SMALL, hIcon);
                        // Don't destroy the icon as it's now owned by the window
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting window icon: {ex.Message}");
            }

            // Set title bar colors
            var titleBar = appWindow.TitleBar;
            titleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 27, 40, 56); // #1B2838
            titleBar.ForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255); // White
            titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 40, 56); // #1B2838
            titleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255); // White
            titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 40, 60, 80); // Slightly lighter blue
            titleBar.ButtonHoverForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255); // White
            titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(255, 60, 80, 100); // Even lighter blue
            titleBar.ButtonPressedForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255); // White

            this.UnhandledException += (_, e) =>
            {
                Debug.WriteLine($"Unhandled UI Exception: {e.Exception.StackTrace}");
                e.Handled = true; // Prevents app from crashing
            };
        }

        private Window m_window;

        // Win32 API imports for setting window icon
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr LoadImage(IntPtr hInst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_SETICON = 0x0080;
        private const int ICON_BIG = 1;
        private const int ICON_SMALL = 0;
        private const uint IMAGE_ICON = 1;
        private const uint LR_LOADFROMFILE = 0x00000010;
    }
}
