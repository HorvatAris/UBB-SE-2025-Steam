namespace SteamHub.ApiContract.Models.User
{
    /// <summary>
    /// Data Transfer Object (DTO) representing user details.
    /// </summary>
    public class User : IUserDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
        }

        /// <summary>
        /// Initializes a new instance with all properties specified.
        /// </summary>
        /// <param name="userIdentifier">Unique identifier for the user.</param>
        /// <param name="name">Display name of the user.</param>
        /// <param name="password">Hashed or plain-text password.</param>
        /// <param name="email">Email address of the user.</param>
        /// <param name="walletBalance">Current wallet balance.</param>
        /// <param name="pointsBalance">Current points balance.</param>
        /// <param name="userRole">Role assigned to the user.</param>
        public User(
            int userIdentifier,
            string name,
            string password,
            string email,
            float walletBalance,
            float pointsBalance,
            UserRole userRole)
        {
            UserId = userIdentifier;
            UserName = name;
            Password = password;
            Email = email;
            WalletBalance = walletBalance;
            PointsBalance = pointsBalance;
            UserRole = userRole;
        }

        /// <summary>
        /// Initializes a new instance by copying properties from another <see cref="IUserDetails"/> instance.
        /// </summary>
        /// <param name="userDetails">The source instance to copy from.</param>
        public User(IUserDetails userDetails)
        {
            UserId = userDetails.UserId;
            UserName = userDetails.UserName;
            Email = userDetails.Email;
            WalletBalance = userDetails.WalletBalance;
            PointsBalance = userDetails.PointsBalance;
            UserRole = userDetails.UserRole;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the display name chosen by the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's password (hashed or plain-text depending on context).
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the balance in the user's wallet (real currency).
        /// </summary>
        public float WalletBalance { get; set; }

        /// <summary>
        /// Gets or sets the user's points balance (redeemable rewards).
        /// </summary>
        public float PointsBalance { get; set; }

        /// <summary>
        /// Gets or sets the user's role (e.g., Developer or User).
        /// </summary>
        public UserRole UserRole { get; set; }
    }
}