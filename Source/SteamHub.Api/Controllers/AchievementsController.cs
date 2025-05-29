
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using System.Threading.Tasks;


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

        [HttpGet]
        public async Task<IActionResult> GetAllAchievements()
        {
            var achievements = await achievementsService.GetAllAchievements();
            return Ok(achievements);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAchievementsForUser(int userId)
        {
            var achievements = await achievementsService.GetAchievementsForUser(userId);
            return Ok(achievements);
        }

        [HttpGet("{userId}/grouped")]
        public async Task<IActionResult> GetGroupedAchievementsForUser(int userId)
        {
            var groupedAchievements = await achievementsService.GetGroupedAchievementsForUser(userId);
            return Ok(groupedAchievements);
        }

        [HttpGet("user/{userId}/status")]
        public async Task<IActionResult> GetAchievementsWithStatusForUser([FromRoute] int userId)
        {
            var achievementsWithStatus =await  achievementsService.GetAchievementsWithStatusForUser(userId);
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
            catch (Exception ex) when (ex.Message.Contains("Achievement is not unlocked or does not exist"))
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