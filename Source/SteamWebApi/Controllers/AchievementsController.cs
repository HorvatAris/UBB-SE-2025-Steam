using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static BusinessLayer.Services.AchievementsService;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementsService achievementsService;

        public AchievementsController(IAchievementsService achievementsService)
        {
            this.achievementsService = achievementsService;
        }

        [HttpGet("{userId}")]
        public IActionResult GetAchievementsForUser(int userId)
        {
            var achievements = achievementsService.GetAchievementsForUser(userId);
            return Ok(achievements);
        }

        [HttpGet("{userId}/grouped")]
        public IActionResult GetGroupedAchievementsForUser(int userId)
        {
            var groupedAchievements = achievementsService.GetGroupedAchievementsForUser(userId);
            return Ok(groupedAchievements);
        }

        [HttpGet("{userId}/status")]
        public IActionResult GetAchievementsWithStatusForUser(int userId)
        {
            var achievementsWithStatus = achievementsService.GetAchievementsWithStatusForUser(userId);
            return Ok(achievementsWithStatus);
        }

        [HttpGet("{userId}/{achievementId}/points")]
        public IActionResult GetPointsForUnlockedAchievement(int userId, int achievementId)
        {
            try
            {
                var points = achievementsService.GetPointsForUnlockedAchievement(userId, achievementId);
                return Ok(points);
            }
            catch (ServiceException ex) when (ex.Message.Contains("Achievement is not unlocked or does not exist"))
            {
                // Return 404 Not Found for achievement not found/unlocked
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Return 500 for unexpected errors
                return StatusCode(500, new { message = "An unexpected error occurred", error = ex.Message });
            }
        }

        [HttpPost("initialize")]
        public IActionResult InitializeAchievements()
        {
            achievementsService.InitializeAchievements();
            return Ok();
        }

        [HttpPost("{userId}/unlock")]
        public IActionResult UnlockAchievementForUser(int userId)
        {
            achievementsService.UnlockAchievementForUser(userId);
            return Ok();
        }

    }
}