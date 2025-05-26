using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly IPasswordResetService passwordResetService;

        public PasswordResetController(IPasswordResetService passwordResetService)
        {
            this.passwordResetService = passwordResetService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendResetCode([FromBody] SendResetCodeRequest request)
        {
            var (isValid, message) = await passwordResetService.SendResetCode(request.Email);
            return Ok(new { IsValid = isValid, Message = message });
        }

        [HttpPost("verify")]
        public IActionResult VerifyResetCode([FromBody] VerifyResetCodeRequest request)
        {
            var (isValid, message) = passwordResetService.VerifyResetCode(request.Email, request.Code);
            return Ok(new { IsValid = isValid, Message = message });
        }

        [HttpPost("reset")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var (isValid, message) = passwordResetService.ResetPassword(request.Email, request.Code, request.NewPassword);
            return Ok(new { IsValid = isValid, Message = message });
        }

        [HttpPost("cleanup")]
        public IActionResult CleanupExpiredCodes()
        {
            passwordResetService.CleanupExpiredCodes();
            return Ok();
        }
    }

    public class SendResetCodeRequest
    {
        public string Email { get; set; }
    }

    public class VerifyResetCodeRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
}