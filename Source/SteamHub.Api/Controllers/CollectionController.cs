using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.Collections;
using SteamHub.ApiContract.Models.Game;


namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectionController : ControllerBase
    {
        private readonly ICollectionsService _collectionsService;

        public CollectionController(ICollectionsService collectionsService)
        {
            _collectionsService = collectionsService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<Collection>>> GetAllCollections(int userId)
        {
            //var result = await _collectionsService.GetAllCollections(userId);
            return null;
            //return Ok(result);
        }

        //[HttpGet("{collectionId}/user/{userId}")]
        //public async Task<ActionResult<Collection>> GetCollectionById(int collectionId, int userId)
        //{
        //    var result = await _collectionsService.GetCollectionByIdentifier(collectionId, userId);
        //    return Ok(result);
        //}

        //[HttpGet("{collectionId}/games")]
        //public async Task<ActionResult<List<OwnedGame>>> GetGamesInCollection(int collectionId)
        //{
        //    var result = await _collectionsService.GetGamesInCollection(collectionId);
        //    return Ok(result);
        //}

        //    [HttpPost("add-game")]
        //    public async Task<ActionResult> AddGameToCollection([FromBody] AddGameRequest request)
        //    {
        //        await _collectionsService.AddGameToCollection(request.CollectionId, request.GameId);
        //        return Ok();
        //    }

        //    [HttpPost("remove-game")]
        //    public async Task<ActionResult> RemoveGameFromCollection([FromBody] RemoveGameRequest request)
        //    {
        //        await _collectionsService.RemoveGameFromCollection(request.CollectionId, request.GameId);
        //        return Ok();
        //    }

        //    [HttpDelete("{collectionId}/user/{userId}")]
        //    public async Task<ActionResult> DeleteCollection(int collectionId, int userId)
        //    {
        //        await _collectionsService.DeleteCollection(collectionId, userId);
        //        return Ok();
        //    }

        //    [HttpPost]
        //    public async Task<ActionResult> CreateCollection([FromBody] Collection collection)
        //    {
        //        await _collectionsService.CreateCollection(
        //            collection.UserId,
        //            collection.CollectionName,
        //            collection.CoverPicture,
        //            collection.IsPublic,
        //            collection.CreatedAt);
        //        return Ok();
        //    }

        //    [HttpPut("{collectionId}")]
        //    public async Task<ActionResult> UpdateCollection(int collectionId, [FromBody] UpdateCollectionRequest request)
        //    {
        //        await _collectionsService.UpdateCollection(
        //            collectionId,
        //            request.UserId,
        //            request.CollectionName,
        //            request.CoverPicture,
        //            request.IsPublic);
        //        return Ok();
        //    }

        //    [HttpGet("public/{userId}")]
        //    public async Task<ActionResult<List<Collection>>> GetPublicCollectionsForUser(int userId)
        //    {
        //        var result = await _collectionsService.GetPublicCollectionsForUser(userId);
        //        return Ok(result);
        //    }

        //    [HttpGet("{collectionId}/user/{userId}/games-not-in-collection")]
        //    public async Task<ActionResult<List<OwnedGame>>> GetGamesNotInCollection(int collectionId, int userId)
        //    {
        //        var result = await _collectionsService.GetGamesNotInCollection(collectionId, userId);
        //        return Ok(result);
        //    }

        //    [HttpGet("user/{userId}/last-three")]
        //    public async Task<ActionResult<List<Collection>>> GetLastThreeCollectionsForUser(int userId)
        //    {
        //        var result = await _collectionsService.GetLastThreeCollectionsForUser(userId);
        //        return Ok(result);
        //    }
        //}

        //// Request DTOs
        //public class AddGameRequest
        //{
        //    public int CollectionId { get; set; }
        //    public int GameId { get; set; }
        //}

        //public class RemoveGameRequest
        //{
        //    public int CollectionId { get; set; }
        //    public int GameId { get; set; }
        //}

        //public class UpdateCollectionRequest
        //{
        //    public int UserId { get; set; }
        //    public string CollectionName { get; set; }
        //    public string CoverPicture { get; set; }
        //    public bool IsPublic { get; set; }
        //}
    }
}