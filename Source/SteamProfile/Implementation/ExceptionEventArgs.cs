using System;

namespace SteamProfile.Implementation
{
    // Used for events (a new exception has been triggered => show it to the user)
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; }
        public ExceptionEventArgs(Exception exception) => Exception = exception;
    }
}
