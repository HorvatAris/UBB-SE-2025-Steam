using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Fakes
{
    public class FakeUserService : IUserService
    {
        public bool AcceptChanges(int userId, string givenPassword)
        {
            return true;
        }

        public User CreateUser(User user)
        {
            var newUser = new User();
            newUser.Username = "fake";
            return newUser;
        }

        public void DeleteUser(int userId)
        {
        }

        public List<User> GetAllUsers()
        {
            List<User> fakeUsers = new List<User>();
            var fakeUser1 = new User();
            fakeUser1.Username = "fake1";
            fakeUsers.Add(fakeUser1);
            var fakeUser2 = new User();
            fakeUser2.Username = "fake2";
            fakeUsers.Add(fakeUser2);
            return fakeUsers;
        }

        public User? GetCurrentUser()
        {
            var fakeUser = new User();
            fakeUser.Username = "fakeCurrent";
            return fakeUser;
        }

        public User GetUserByEmail(string email)
        {
            // Only return a user for specific test emails
            if (email == "user1@example.com" || email == "user2@example.com")
            {
                var fakeUser = new User();
                fakeUser.Username = "fakeMail";
                fakeUser.Email = email;
                return fakeUser;
            }
            return null;
        }

        public User GetUserByIdentifier(int userId)
        {
            var fakeUser = new User();
            fakeUser.Username = "fakeId";
            return fakeUser;
        }

        public User GetUserByUsername(string username)
        {
            var fakeUser = new User();
            fakeUser.Username = "fake";
            return fakeUser;
        }

        public bool IsUserLoggedIn()
        {
            return true;
        }

        public User? Login(string emailOrUsername, string password)
        {
            var fakeUser = new User();
            fakeUser.Username = "fake";
            return fakeUser;
        }

        public void Logout()
        {
        }

        public User UpdateUser(User user)
        {
            var fakeUser = new User();
            fakeUser.Username = "fake";
            return fakeUser;
        }

        public void UpdateUserEmail(int userId, string newEmail)
        {
        }

        public bool UpdateUserEmail(string email, string currentPassword)
        {
            return true;
        }

        public void UpdateUserPassword(int userId, string newPassword)
        {
        }

        public bool UpdateUserPassword(string password, string currentPassword)
        {
            return true;
        }

        public void UpdateUserUsername(int userId, string newUsername)
        {
        }

        public bool UpdateUserUsername(string username, string currentPassword)
        {
            return true;
        }

        public void ValidateUserAndEmail(string email, string username)
        {
            // throw an error maybe for later tests
        }

        public bool VerifyUserPassword(string password)
        {
            return true;
        }
    }
}
