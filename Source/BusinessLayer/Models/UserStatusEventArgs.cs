using System;

namespace BusinessLayer.Models
{
    /// <summary>
    /// Event arguments for user status change events.
    /// </summary>
    public class UserStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the current status of the user.
        /// </summary>
        public UserStatus UserStatus { get; }

        /// <summary>
        /// Initializes a new instance of the UserStatusEventArgs class.
        /// </summary>
        /// <param name="userStatus">The user status to encapsulate.</param>
        public UserStatusEventArgs(UserStatus userStatus) =>
            UserStatus = userStatus ?? throw new ArgumentNullException(nameof(userStatus));
    }
}