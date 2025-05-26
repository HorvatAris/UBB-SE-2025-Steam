using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User GetUserByIdentifier(int userIdentifier);
        User GetUserByEmail(string email);
        User GetUserByUsername(string username);
        void ValidateUserAndEmail(string email, string username);
        User CreateUser(User user);
        User UpdateUser(User user);
        void DeleteUser(int userIdentifier);
        bool AcceptChanges(int userIdentifier, string givenPassword);
        void UpdateUserEmail(int userIdentifier, string newEmail);
        void UpdateUserPassword(int userIdentifier, string newPassword);
        void UpdateUserUsername(int userIdentifier, string newUsername);
        User? Login(string emailOrUsername, string password);
        void Logout();
        User GetCurrentUser();
        bool IsUserLoggedIn();
        bool UpdateUserUsername(string username, string currentPassword);
        bool UpdateUserPassword(string password, string currentPassword);
        bool UpdateUserEmail(string email, string currentPassword);
        bool VerifyUserPassword(string password);
    }
}
