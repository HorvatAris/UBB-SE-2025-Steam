using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        List<User> GetAllUsers();
        User? GetUserById(int userId);
        User UpdateUser(User user);
        User CreateUser(User user);
        void DeleteUser(int userId);
        User? VerifyCredentials(string emailOrUsername);
        User? GetUserByEmail(string email);
        User? GetUserByUsername(string username);
        string CheckUserExists(string email, string username);
        void ChangeEmail(int userId, string newEmail);
        void ChangePassword(int userId, string newPassword);
        void ChangeUsername(int userId, string newUsername);
        void UpdateLastLogin(int userId);
    }
}
