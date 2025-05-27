using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessLayer.Models;
using BusinessLayer.Models.Login;
using BusinessLayer.Models.Register;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Repositories.Interfaces;
using LoginResponse = BusinessLayer.Services.Proxies.LoginResponse;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ISessionService sessionService;
        private readonly IConfiguration configuration;
        private readonly IWalletService walletService;
        private readonly IUserProfilesRepository profilesRepo;

        /// <summary>
        /// Constructor for AuthController
        /// </summary>
        /// <param name="userService">Service to manage user operations</param>
        /// <param name="sessionService">Service to manage session operations</param>
        /// <param name="configuration">Application configuration settings</param>
        public AuthController(IUserService userService, ISessionService sessionService, IConfiguration configuration, IWalletService walletService, IUserProfilesRepository userProfilesRepository)
        {
            this.userService = userService;
            this.sessionService = sessionService;
            this.configuration = configuration;
            this.walletService = walletService;
            this.profilesRepo = userProfilesRepository;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token and session information.
        /// </summary>
        /// <param name="request">Login request containing email/username and password</param>
        /// <returns>JWT token and session details if successful, appropriate error otherwise</returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.EmailOrUsername) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username or email, and password are required.");
            }

            // Authenticate the user using the user service
            var user = userService.Login(request.EmailOrUsername, request.Password);

            if (user == null)
            {
                return Unauthorized(); // Invalid credentials
            }

            // Retrieve the session ID from the UserSession singleton
            var sessionId = UserSession.Instance.CurrentSessionId.Value;

            // Construct session detail object
            var userWithSessionDetails = new UserWithSessionDetails
            {
                SessionId = sessionId,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Developer = user.IsDeveloper,
                UserCreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                CreatedAt = UserSession.Instance.CreatedAt,
                ExpiresAt = UserSession.Instance.ExpiresAt
            };

            // Generate a JWT token
            var token = GenerateJwtToken(user, sessionId);

            // Return successful login response
            return Ok(new LoginResponse
            {
                User = user,
                Token = token,
                UserWithSessionDetails = userWithSessionDetails
            });
        }

        /// <summary>
        /// Registers a new user and returns a JWT token and session information.
        /// </summary>
        /// <param name="request">Register request containing username, email, password, and developer flag</param>
        /// <returns>JWT token and session details if successful, appropriate error otherwise</returns>
        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Email) || 
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username, email and password are required.");
            }

            // Attempt to create the user
            User newUser;
            try
            {
                newUser = userService.CreateUser(new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    IsDeveloper = request.IsDeveloper
                });
                walletService.CreateWallet(newUser.UserId);
            }
            catch (Exception ex)
            {
                // User creation failed
                return BadRequest(ex.Message); 
            }

            // Create a new session for the user
            var sessionId = sessionService.CreateNewSession(newUser);

            // Build session detail response object
            var details = new UserWithSessionDetails
            {
                SessionId = sessionId,
                UserId = newUser.UserId,
                Username = newUser.Username,
                Email = newUser.Email,
                Developer = newUser.IsDeveloper,
                UserCreatedAt = newUser.CreatedAt,
                LastLogin = newUser.LastLogin,
                CreatedAt = UserSession.Instance.CreatedAt,
                ExpiresAt = UserSession.Instance.ExpiresAt
            };

            // Generate a JWT token
            var token = GenerateJwtToken(newUser, sessionId);

            // Return successful registration response
            return Ok(new RegisterResponse
            {
                User = newUser,
                Token = token,
                UserWithSessionDetails = details
            });
        }

        /// <summary>
        /// Generates a JWT token for the authenticated user.
        /// </summary>
        /// <param name="user">User object</param>
        /// <param name="sessionId">Current session ID</param>
        /// <returns>JWT token as a string</returns>
        private string GenerateJwtToken(User user, Guid sessionId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration["Jwt:Key"] ?? "YourTemporarySecretKeyHereMustBe32Chars"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Set user claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.Username),
                new Claim("sessionId", sessionId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Create the token
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"] ?? "SteamWebApi",
                audience: configuration["Jwt:Audience"] ?? "SteamProfile",
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials);

            // Return the serialized token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
