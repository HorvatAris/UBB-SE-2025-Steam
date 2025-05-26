using System;
using System.Collections.Generic;
using System.Diagnostics;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Proxies
{
    public class UserServiceProxy : ServiceProxy, IUserService
    {
        private readonly ISessionService sessionService;

        public UserServiceProxy(ISessionService sessionService, string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        public List<User> GetAllUsers()
        {
            try
            {
                // Execute the async call synchronously
                return GetAsync<List<User>>("User").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                // If there's an error, return an empty list
                return new List<User>();
            }
        }

        public User GetUserByIdentifier(int userId)
        {
            try
            {
                return GetAsync<User>($"User/{userId}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public User GetUserByEmail(string email)
        {
            try
            {
                return GetAsync<User>($"User/email/{email}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public User GetUserByUsername(string username)
        {
            try
            {
                return GetAsync<User>($"User/username/{username}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ValidateUserAndEmail(string email, string username)
        {
            try
            {
                PostSync("User/validate", new { Email = email, Username = username });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ValidateUserAndEmail Exception: {ex.Message}");
                throw new BusinessLayer.Exceptions.UserValidationException(ex.Message);
            }
        }

        public User CreateUser(User user)
        {
            try
            {
                return PostSync<User>("User", user);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateUser Exception: {ex.Message}");
                Debug.WriteLine($"Exception Type: {ex.GetType().FullName}");

                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Debug.WriteLine($"Inner Exception Type: {ex.InnerException.GetType().FullName}");
                }

                throw new BusinessLayer.Exceptions.UserValidationException($"Failed to create user: {ex.Message}");
            }
        }

        public User UpdateUser(User user)
        {
            try
            {
                return PutAsync<User>($"User/{user.UserId}", user).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new BusinessLayer.Exceptions.ServiceException($"Failed to update user: {ex.Message}", ex);
            }
        }

        public void DeleteUser(int userId)
        {
            try
            {
                DeleteAsync<object>($"User/{userId}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new BusinessLayer.Exceptions.ServiceException($"Failed to delete user: {ex.Message}", ex);
            }
        }

        public bool AcceptChanges(int userId, string givenPassword)
        {
            try
            {
                return PostAsync<bool>($"User/{userId}/verify", new { Password = givenPassword }).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void UpdateUserEmail(int userId, string newEmail)
        {
            try
            {
                PutAsync<User>($"User/{userId}/email", new { Email = newEmail }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new BusinessLayer.Exceptions.ServiceException($"Failed to update email: {ex.Message}", ex);
            }
        }

        public void UpdateUserPassword(int userId, string newPassword)
        {
            try
            {
                PutAsync<User>($"User/{userId}/password", new { Password = newPassword }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new BusinessLayer.Exceptions.ServiceException($"Failed to update password: {ex.Message}", ex);
            }
        }

        public void UpdateUserUsername(int userId, string newUsername)
        {
            try
            {
                PutAsync<User>($"User/{userId}/username", new { Username = newUsername }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new BusinessLayer.Exceptions.ServiceException($"Failed to update username: {ex.Message}", ex);
            }
        }

        public User Login(string emailOrUsername, string password)
        {
            try
            {
                var loginResponse = PostAsync<LoginResponse>("Auth/Login", new
                {
                    EmailOrUsername = emailOrUsername,
                    Password = password
                }).GetAwaiter().GetResult();

                if (loginResponse != null && loginResponse.User != null)
                {
                    // Store the auth token
                    SetAuthToken(loginResponse.Token);

                    // Store the user with session details
                    SetCurrentUser(loginResponse.UserWithSessionDetails);

                    return loginResponse.User;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
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
            try
            {
                PostAsync("User/updateUsername", new
                {
                    Username = username,
                    CurrentPassword = currentPassword
                }).GetAwaiter().GetResult();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUserPassword(string password, string currentPassword)
        {
            try
            {
                PostAsync("User/updatePassword", new
                {
                    Password = password,
                    CurrentPassword = currentPassword
                }).GetAwaiter().GetResult();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUserEmail(string email, string currentPassword)
        {
            try
            {
                PostAsync("User/updateEmail", new
                {
                    Email = email,
                    CurrentPassword = currentPassword
                }).GetAwaiter().GetResult();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool VerifyUserPassword(string password)
        {
            try
            {
                return PostAsync<bool>("User/verifyPassword", new { Password = password }).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }
    }

    // Modified LoginResponse class
    public class LoginResponse
    {
        public User User { get; set; }
        public string Token { get; set; }
        public UserWithSessionDetails UserWithSessionDetails { get; set; }
    }
}