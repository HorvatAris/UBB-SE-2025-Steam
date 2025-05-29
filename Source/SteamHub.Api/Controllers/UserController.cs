﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.User;
using SteamHub.Api.Context;
using SteamHub.ApiContract.Exceptions;

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
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await userService.GetAllUsersAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await userService.GetUserByIdentifierAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await userService.GetUserByEmailAsync(email);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await userService.GetUserByUsernameAsync(username);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateUser([FromBody] ValidateUserRequest request)
        {
            try
            {
                await userService.ValidateUserAndEmailAsync(request.Email, request.Username);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                var createdUser = await userService.CreateUserAsync(user);
                return Ok(createdUser);
            }
            catch (EmailAlreadyExistsException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UsernameAlreadyTakenException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Failed to create user" });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.UserId)
            {
                return BadRequest("User ID mismatch");
            }

            try
            {
                var updatedUser = await userService.UpdateUserAsync(user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await userService.DeleteUserAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/verify")]
        [Authorize]
        public async Task<IActionResult> VerifyPassword(int id, [FromBody] PasswordVerifyRequest request)
        {
            return Ok(await userService.AcceptChangesAsync(id, request.Password));
        }

        [HttpPut("{id}/email")]
        [Authorize]
        public async Task<IActionResult> UpdateEmail(int id, [FromBody] EmailUpdateRequest request)
        {
            try
            {
                await userService.UpdateUserEmailAsync(id, request.Email);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] PasswordUpdateRequest request)
        {
            try
            {
                await userService.UpdateUserPasswordAsync(id, request.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/username")]
        [Authorize]
        public async Task<IActionResult> UpdateUsername(int id, [FromBody] UsernameUpdateRequest request)
        {
            try
            {
                await userService.UpdateUserUsernameAsync(id, request.Username);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("updateUsername")]
        [Authorize]
        public async Task<IActionResult> UpdateUsername([FromBody] UsernameChangeRequest request)
        {
            var user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await userService.UpdateUserUsernameAsync(request.Username, request.CurrentPassword);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to update username");
        }

        [HttpPost("updatePassword")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordChangeRequest request)
        {
            var user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await userService.UpdateUserPasswordAsync(request.Password, request.CurrentPassword);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to update password");
        }

        [HttpPost("updateEmail")]
        [Authorize]
        public async Task<IActionResult> UpdateEmail([FromBody] EmailChangeRequest request)
        {
            var user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await userService.UpdateUserEmailAsync(request.Email, request.CurrentPassword);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to update email");
        }

        [HttpPost("updatePFP")]
        public async Task<IActionResult> UpdatePFP([FromBody] PFPChangeRequest request)
        {
            var user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await userService.UpdateProfilePictureAsync(user.UserId, request.ProfilePicture);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to update pfp");
        }

        [HttpPost("updateBio")]
        public async Task<IActionResult> UpdateBio([FromBody] BioChangeRequest request)
        {
            var user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await userService.UpdateProfileBioAsync(user.UserId, request.Bio);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to update bio");
        }

        [HttpPost("verifyPassword")]
        [Authorize]
        public async Task<IActionResult> VerifyPassword([FromBody] PasswordVerifyRequest request)
        {
            return Ok(await userService.VerifyUserPasswordAsync(request.Password));
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

    public class PFPChangeRequest
    {
        public string ProfilePicture { get; set; }
    }

    public class BioChangeRequest
    {
        public string Bio { get; set; }
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

