using System;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Helpers
{
    /// <summary>
    /// Helper class to store navigation-related callbacks and state.
    /// </summary>
    public static class NavigationHelper
    {
        /// <summary>
        /// Gets or sets the callback to be called when login is successful.
        /// </summary>
        public static Action<User> OnLoginSuccess { get; set; }
    }
} 