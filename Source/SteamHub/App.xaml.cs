﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.User;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SteamHub
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
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

        public static User CurrentUser { get; set; }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();
            //var rootFrame = new Frame();
            //MainWindow.Content = rootFrame;
            //rootFrame.Navigate(typeof(LoginPage), null);
            MainWindow.Activate();
            this.UnhandledException += (_, e) =>
            {
                Debug.WriteLine($"Unhandled UI Exception: {e.Exception.StackTrace}");
                e.Handled = true; // Prevents app from crashing
            };

        }

    }

}
