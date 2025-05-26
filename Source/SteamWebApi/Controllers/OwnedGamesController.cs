using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnedGamesController : ControllerBase
    {
        private readonly IOwnedGamesService ownedGamesService;

        public OwnedGamesController(IOwnedGamesService ownedGamesService)
        {
            this.ownedGamesService = ownedGamesService;
        }

        [HttpGet("{userId}")]
        public IActionResult GetAllOwnedGames(int userId)
        {
            var ownedGames = ownedGamesService.GetAllOwnedGames(userId);
            return Ok(ownedGames);
        }

        [HttpGet("{userId}/game/{gameId}")]
        public IActionResult GetOwnedGameByIdentifier(int userId, int gameId)
        {
            var ownedGame = ownedGamesService.GetOwnedGameByIdentifier(gameId, userId);
            if (ownedGame == null)
            {
                return NotFound();
            }
            return Ok(ownedGame);
        }

        [HttpDelete("{userId}/game/{gameId}")]
        public IActionResult RemoveOwnedGame(int userId, int gameId)
        {
            ownedGamesService.RemoveOwnedGame(gameId, userId);
            return Ok();
        }
    }
}