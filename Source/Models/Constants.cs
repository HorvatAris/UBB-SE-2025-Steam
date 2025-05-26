// BusinessLayer/Models/Constants.cs
// New file to store all constants used in the chat system
using System;

namespace BusinessLayer.Models
{
    /// <summary>
    /// Contains constant values used throughout the chat system.
    /// </summary>
    public static class ChatConstants
    {
        public const bool DEBUG_MODE = true;
        public const string DEBUG_HOST_IP = "127.0.0.1";
        public const string DEBUG_CLIENT1_IP = "127.0.0.2";
        public const string DEBUG_CLIENT2_IP = "127.0.0.3";

        // Network constants
        public const int PORT_NUMBER = 6000;
        public const int MESSAGE_MAXIMUM_SIZE = 4112;
        public const int USER_NAME_MAXIMUM_SIZE = 512;
        public const int MAXIMUM_NUMBER_OF_ACTIVE_CONNECTIONS = 20;
        public const int NUMBER_OF_QUEUED_CONNECTIONS = 10;
        public const int STARTING_INDEX = 0;
        public const int DISCONNECT_CODE = 0;
        public const int SERVER_TIMEOUT_DURATION_MS = 180000; // 3 minutes
        public const int MINIMUM_CONNECTIONS_REQUIRED = 2;

        // Status constants
        public const string ADMIN_STATUS = "ADMIN";
        public const string MUTE_STATUS = "MUTE";
        public const string KICK_STATUS = "KICK";
        public const string HOST_STATUS = "HOST";
        public const string REGULAR_USER_STATUS = "USER";

        // Command patterns
        public const string INFO_CHANGE_MUTE_STATUS_COMMAND = "<INFO>|" + MUTE_STATUS + "|<INFO>";
        public const string INFO_CHANGE_ADMIN_STATUS_COMMAND = "<INFO>|" + ADMIN_STATUS + "|<INFO>";
        public const string INFO_CHANGE_KICK_STATUS_COMMAND = "<INFO>|" + KICK_STATUS + "|<INFO>";

        // Server messages
        public const string SERVER_REJECT_COMMAND = "Server rejected your command!\n You don't have the rights to that user!";
        public const string SERVER_CAPACITY_REACHED = "Server capacity reached!\n Closing Connection!";

        // IP address constants
        public const string HOST_IP_FINDER = "None";
        public const string GET_IP_REPLACER = "NULL";
        public const char ADDRESS_SEPARATOR = ':';

        // UI constants
        public const int MAX_MESSAGES_TO_DISPLAY = 100;
        public const int CONNECTION_CHECK_DELAY_MS = 50;

        // Message alignment constants
        public const string ALIGNMENT_LEFT = "Left";
        public const string ALIGNMENT_RIGHT = "Right";
    }
}