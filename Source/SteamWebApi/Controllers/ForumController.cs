using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForumController : ControllerBase
    {
        private readonly IForumService forumService;

        public ForumController(IForumService forumService)
        {
            this.forumService = forumService;
        }

        [HttpGet("posts")]
        public IActionResult GetPosts(
            [FromQuery] uint page = 0,
            [FromQuery] uint size = 10,
            [FromQuery] bool positiveOnly = false,
            [FromQuery] int? gameId = null,
            [FromQuery] string? filter = null)
        {
            var posts = forumService.GetPagedPosts(page, size, positiveOnly, gameId, filter);
            return Ok(posts);
        }

        [HttpGet("top-posts")]
        public IActionResult GetTopPosts([FromQuery] TimeSpanFilter filter = TimeSpanFilter.AllTime)
        {
            var posts = forumService.GetTopPosts(filter);
            return Ok(posts);
        }

        [HttpGet("posts/{postId}/comments")]
        public IActionResult GetComments(int postId)
        {
            var comments = forumService.GetComments(postId);
            return Ok(comments);
        }

        [HttpPost("posts")]
        public IActionResult CreatePost([FromBody] CreatePostRequest request)
        {
            forumService.CreatePost(request.Title, request.Body, request.Date, request.GameId);
            return Ok();
        }

        [HttpDelete("posts/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            forumService.DeletePost(postId);
            return Ok();
        }

        [HttpPost("posts/{postId}/vote")]
        public IActionResult VoteOnPost(int postId, [FromBody] ForumVoteRequest request)
        {
            forumService.VoteOnPost(postId, request.VoteValue);
            return Ok();
        }

        [HttpPost("comments")]
        public IActionResult CreateComment([FromBody] CreateCommentRequest request)
        {
            forumService.CreateComment(request.Body, request.PostId, request.Date);
            return Ok();
        }

        [HttpDelete("comments/{commentId}")]
        public IActionResult DeleteComment(int commentId)
        {
            forumService.DeleteComment(commentId);
            return Ok();
        }

        [HttpPost("comments/{commentId}/vote")]
        public IActionResult VoteOnComment(int commentId, [FromBody] ForumVoteRequest request)
        {
            forumService.VoteOnComment(commentId, request.VoteValue);
            return Ok();
        }
    }

    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Date { get; set; }
        public int? GameId { get; set; }
    }

    public class CreateCommentRequest
    {
        public string Body { get; set; }
        public int PostId { get; set; }
        public string Date { get; set; }
    }

    public class ForumVoteRequest
    {
        public int VoteValue { get; set; }
    }
}