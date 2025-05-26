namespace BusinessLayer.Models
{
    /// <summary>
    /// Represents the current status of a user in the chat system.
    /// </summary>
    public class UserStatus
    {
        private bool isAdmin;
        private bool isMuted;
        private bool isHost;
        private bool isConnected;

        /// <summary>
        /// Gets or sets whether the user has admin privileges.
        /// </summary>
        public bool IsAdmin { get => isAdmin; set => isAdmin = value; }

        /// <summary>
        /// Gets or sets whether the user is currently muted.
        /// </summary>
        public bool IsMuted { get => isMuted; set => isMuted = value; }

        /// <summary>
        /// Gets or sets whether the user is the host of the chat room.
        /// </summary>
        public bool IsHost { get => isHost; set => isHost = value; }

        /// <summary>
        /// Gets or sets whether the user is currently connected to the chat room.
        /// </summary>
        public bool IsConnected { get => isConnected; set => isConnected = value; }

        /// <summary>
        /// Initializes a new instance of the UserStatus class with default values.
        /// </summary>
        public UserStatus()
        {
            this.isAdmin = false;
            this.isMuted = false;
            this.isHost = false;
            this.isConnected = false;
        }
    }
}