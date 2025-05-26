namespace SteamProfile.Implementation
{
    // Client Status is used to new events
    public class ClientStatus
    {
        private bool isAdmin;
        private bool isMuted;
        private bool isHost;
        private bool isConnected;

        public bool IsAdmin { get => isAdmin; set => isAdmin = value; }
        public bool IsMuted { get => isMuted; set => isMuted = value; }
        public bool IsHost { get => isHost; set => isHost = value; }
        public bool IsConnected { get => isConnected; set => isConnected = value; }

        public ClientStatus()
        {
            this.isAdmin = this.IsMuted = this.IsHost = this.IsConnected = false;
        }
    }
}
