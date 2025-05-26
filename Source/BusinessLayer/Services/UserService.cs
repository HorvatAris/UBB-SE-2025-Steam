using BusinessLayer.Models;
using BusinessLayer.Exceptions;
using BusinessLayer.Utils;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository usersRepository;
        private readonly ISessionService sessionService;

        public UserService(IUsersRepository usersRepository, ISessionService sessionService)
        {
            this.usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        public List<User> GetAllUsers()
        {
            return usersRepository.GetAllUsers();
        }

        public User GetUserByIdentifier(int userId) => usersRepository.GetUserById(userId);

        public User GetUserByEmail(string email) => usersRepository.GetUserByEmail(email);

        public User? GetUserByUsername(string username) => usersRepository.GetUserByUsername(username);
        public void ValidateUserAndEmail(string email, string username)
        {
            // Check if user already exists
            var errorType = usersRepository.CheckUserExists(email, username);

            if (!string.IsNullOrEmpty(errorType))
            {
                switch (errorType)
                {
                    case "EMAIL_EXISTS":
                        throw new EmailAlreadyExistsException(email);
                    case "USERNAME_EXISTS":
                        throw new UsernameAlreadyTakenException(username);
                    default:
                        throw new UserValidationException($"Unknown validation error: {errorType}");
                }
            }
        }

        public User CreateUser(User user)
        {
            ValidateUserAndEmail(user.Email, user.Username);

            // Hash the password before passing it to the repository
            user.Password = PasswordHasher.HashPassword(user.Password);
            return usersRepository.CreateUser(user);
        }

        public User UpdateUser(User user)
        {
            return usersRepository.UpdateUser(user);
        }

        public void DeleteUser(int userId)
        {
            usersRepository.DeleteUser(userId);
        }

        public bool AcceptChanges(int user_id, string givenPassword)
        {
            User user = usersRepository.GetUserById(user_id);

            if (PasswordHasher.VerifyPassword(givenPassword, user.Password))
            {
                return true;
            }
            return false;
        }

        public void UpdateUserEmail(int userId, string newEmail)
        {
            usersRepository.ChangeEmail(userId, newEmail);
        }
        public void UpdateUserPassword(int userId, string newPassword)
        {
            usersRepository.ChangePassword(userId, newPassword);
        }
        public void UpdateUserUsername(int userId, string newUsername)
        {
            usersRepository.ChangeUsername(userId, newUsername);
        }

        public User? Login(string emailOrUsername, string password)
        {
            var user = usersRepository.VerifyCredentials(emailOrUsername);

            /*bool bypassLogin = true;
            if (bypassLogin)
            {
                sessionService.CreateNewSession(user);
                usersRepository.UpdateLastLogin(user.UserId);
                return user;
            }*/

            if (user != null)
            {
                if (PasswordHasher.VerifyPassword(password, user.Password))
                {
                    sessionService.CreateNewSession(user);

                    // update last login time for user
                    usersRepository.UpdateLastLogin(user.UserId);
                }
                else
                {
                    return null;
                }
            }

            return user;
        }

        public void Logout()
        {
            sessionService.EndSession();
        }

        public User GetCurrentUser()
        {
            return sessionService.GetCurrentUser();
        }

        public bool IsUserLoggedIn()
        {
            return sessionService.IsUserLoggedIn();
        }

        public bool UpdateUserUsername(string username, string currentPassword)
        {
            if (this.VerifyUserPassword(currentPassword))
            {
                usersRepository.ChangeUsername(GetCurrentUser().UserId, username);
                return true;
            }
            return false;
        }

        public bool UpdateUserPassword(string password, string currentPassword)
        {
            if (this.VerifyUserPassword(currentPassword))
            {
                usersRepository.ChangePassword(GetCurrentUser().UserId, password);
                return true;
            }
            return false;
        }

        public bool UpdateUserEmail(string email, string currentPassword)
        {
            if (this.VerifyUserPassword(currentPassword))
            {
                usersRepository.ChangeEmail(GetCurrentUser().UserId, email);
                return true;
            }
            return false;
        }

        public bool VerifyUserPassword(string password)
        {
            string email = this.GetCurrentUser().Email;
            var user = usersRepository.VerifyCredentials(email);
            if (user != null)
            {
                if (PasswordHasher.VerifyPassword(password, user.Password))
                {
                    return true;
                }
            }
            return false;
        }
    }
}