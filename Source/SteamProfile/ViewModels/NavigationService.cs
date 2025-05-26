using System;
using Microsoft.UI.Xaml.Controls;

namespace SteamProfile.ViewModels
{
    public class NavigationService
    {
        private Frame navigationServiceFrame;
        private static NavigationService navigationServiceInstance;

        public static NavigationService Instance
        {
            get
            {
                if (navigationServiceInstance == null)
                {
                    navigationServiceInstance = new NavigationService();
                }
                return navigationServiceInstance;
            }
        }

        public void Initialize(Frame frame)
        {
            navigationServiceFrame = frame;
        }

        public bool Navigate(Type pageType)
        {
            if (navigationServiceFrame != null)
            {
                return navigationServiceFrame.Navigate(pageType);
            }
                return false;

            return navigationServiceFrame.Navigate(pageType);
        }

        public bool Navigate(Type pageType, object parameter)
        {
            if (navigationServiceFrame != null)
        {
                return navigationServiceFrame.Navigate(pageType, parameter);
            }
                return false;

            return navigationServiceFrame.Navigate(pageType, parameter);
        }

        public bool GoBack()
        {
            if (navigationServiceFrame != null && navigationServiceFrame.CanGoBack)
        {
            if (navigationServiceFrame == null || !navigationServiceFrame.CanGoBack)
            {
                return false;
            }

            navigationServiceFrame.GoBack();
            return true;
        }
            return false;
        }
    }
}