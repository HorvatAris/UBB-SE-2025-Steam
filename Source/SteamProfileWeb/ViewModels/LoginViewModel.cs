using System.ComponentModel.DataAnnotations;

namespace SteamProfileWeb.ViewModels
{
    /// <summary>
    /// ViewModel for user login, contains credentials and validation attributes.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// The username or email address used to log in.
        /// </summary>
        [Required(ErrorMessage = "Username or email is required.")]
        [Display(Name = "Username or Email")]
        public string UsernameOrEmail { get; set; }

        /// <summary>
        /// The password for the account.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}