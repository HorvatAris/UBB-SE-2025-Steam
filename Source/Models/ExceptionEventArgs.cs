using System;

namespace BusinessLayer.Models
{
    /// <summary>
    /// Event arguments for exception-related events.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the exception associated with the event.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Initializes a new instance of the ExceptionEventArgs class.
        /// </summary>
        /// <param name="exception">The exception to encapsulate.</param>
        public ExceptionEventArgs(Exception exception) =>
           Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }
}