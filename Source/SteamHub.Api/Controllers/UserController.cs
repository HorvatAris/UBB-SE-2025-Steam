using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.User;


using SteamHub.Api.Context;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly DataContext dbContext;

        public UserController(IUserService userService, DataContext dbContext)
        {
            this.userService = userService;
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAllUsers()
        {
            return Ok(userService.GetAllUsers());
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = userService.GetUserByIdentifier(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public IActionResult GetUserByEmail(string email)
        {
            var user = userService.GetUserByEmail(email);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("username/{username}")]
        public IActionResult GetUserByUsername(string username)
        {
            var user = userService.GetUserByUsername(username);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("validate")]
        public IActionResult ValidateUser([FromBody] ValidateUserRequest request)
        {
            try
            {
                userService.ValidateUserAndEmail(request.Email, request.Username);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*
    [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();

            return strategy.ExecuteAsync(async () =>
            {
                using var transaction = await dbContext.Database.BeginTransactionAsync();
                try
                {
                    var createdUser = userService.CreateUser(user);
                    if (createdUser == null || createdUser.UserId <= 0)
                    {
                        throw new Exception("User creation failed or returned an invalid user.");
                    }

                    walletService.CreateWallet(createdUser.UserId);

                    await transaction.CommitAsync();
                    return Ok(createdUser);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(ex.Message);
                }
            }).Result; // Ensure the result is awaited properly
        }
        */
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.UserId)
            {
                return BadRequest("User ID mismatch");
            }

            try
            {
                var updatedUser = userService.UpdateUser(user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                userService.DeleteUser(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/verify")]
        [Authorize]
        public IActionResult VerifyPassword(int id, [FromBody] PasswordVerifyRequest request)
        {
            return Ok(userService.AcceptChanges(id, request.Password));
        }

        [HttpPut("{id}/email")]
        [Authorize]
        public IActionResult UpdateEmail(int id, [FromBody] EmailUpdateRequest request)
        {
            try
            {
                userService.UpdateUserEmail(id, request.Email);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/password")]
        [Authorize]
        public IActionResult UpdatePassword(int id, [FromBody] PasswordUpdateRequest request)
        {
            try
            {
                userService.UpdateUserPassword(id, request.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/username")]
        [Authorize]
        public IActionResult UpdateUsername(int id, [FromBody] UsernameUpdateRequest request)
        {
            try
            {
                userService.UpdateUserUsername(id, request.Username);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("updateUsername")]
        [Authorize]
        public IActionResult UpdateUsername([FromBody] UsernameChangeRequest request)
        {
            var user = userService.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = userService.UpdateUserUsername(request.Username, request.CurrentPassword);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to update username");
        }

        [HttpPost("updatePassword")]
        [Authorize]
        public IActionResult UpdatePassword([FromBody] PasswordChangeRequest request)
        {
            var user = userService.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = userService.UpdateUserPassword(request.Password, request.CurrentPassword);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to update password");
        }

        [HttpPost("updateEmail")]
        [Authorize]
        public IActionResult UpdateEmail([FromBody] EmailChangeRequest request)
        {
            var user = userService.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = userService.UpdateUserEmail(request.Email, request.CurrentPassword);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to update email");
        }

        [HttpPost("verifyPassword")]
        [Authorize]
        public IActionResult VerifyPassword([FromBody] PasswordVerifyRequest request)
        {
            return Ok(userService.VerifyUserPassword(request.Password));
        }
    }

    // Request DTOs
    public class ValidateUserRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
    }

    public class PasswordVerifyRequest
    {
        public string Password { get; set; }
    }

    public class EmailUpdateRequest
    {
        public string Email { get; set; }
    }

    public class PasswordUpdateRequest
    {
        public string Password { get; set; }
    }

    public class UsernameUpdateRequest
    {
        public string Username { get; set; }
    }

    public class UsernameChangeRequest
    {
        public string Username { get; set; }
        public string CurrentPassword { get; set; }
    }

    public class PasswordChangeRequest
    {
        public string Password { get; set; }
        public string CurrentPassword { get; set; }
    }

    public class EmailChangeRequest
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
    }
}

