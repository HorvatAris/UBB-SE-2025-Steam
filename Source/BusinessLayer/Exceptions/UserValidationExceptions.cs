using System;

namespace BusinessLayer.Exceptions
{
    public class EmailAlreadyExistsException : Exception
    {
        private const string MessageTemplate = "An account with the email '{0}' already exists.";

        public EmailAlreadyExistsException(string email)
            : base(string.Format(MessageTemplate, email))
        {
        }
    }

    public class UsernameAlreadyTakenException : Exception
    {
        private const string MessageTemplate = "The username '{0}' is already taken.";

        public UsernameAlreadyTakenException(string username)
            : base(string.Format(MessageTemplate, username))
        {
        }
    }

    public class UserValidationException : Exception
    {
        public UserValidationException(string message)
            : base(message)
        {
        }
    }
}