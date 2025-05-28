using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService newsService;

        public NewsController(INewsService newsService)
        {
            this.newsService = newsService;
        }

        [HttpGet("{userId}/posts")]
        public async Task<IActionResult> GetPostsAsync([FromRoute] int userId, [FromQuery] int page = 0, [FromQuery] string search = "")
        {
            var posts = await newsService.LoadNextPostsAsync(page, search, userId);
            return Ok(posts);
        }

        [HttpGet("{userId}/posts/{postId}/comments")]
        public async Task<IActionResult> GetCommentsAsync([FromRoute] int postId)
        {
            var comments = await newsService.LoadNextCommentsAsync(postId);
            return Ok(comments);
        }

        [HttpPost("format")]
        public async Task<IActionResult> FormatAsPostAsync([FromBody] FormatRequest request)
        {
            var formattedText = await newsService.FormatAsPostAsync(request.Text);
            return Ok(formattedText);
        }

        [HttpPost("{userId}/posts")]
        public async Task<IActionResult> SavePostAsync([FromBody] ContentRequest request, [FromRoute] int userId)
        {
            var result = await newsService.SavePostAsync(request.Content, userId);
            return Ok(result);
        }

        [HttpPut("{userId}/posts/{postId}")]
        public async Task<IActionResult> UpdatePostAsync([FromRoute] int postId, [FromBody] ContentRequest request)
        {
            var result = await newsService.UpdatePostAsync(postId, request.Content);
            return Ok(result);
        }

        [HttpDelete("{userId}/posts/{postId}")]
        public async Task<IActionResult> DeletePostAsync([FromRoute] int postId)
        {
            var result = await newsService.DeletePostAsync(postId);
            return Ok(result);
        }

        [HttpPost("{userId}/posts/{postId}/like")]
        public async Task<IActionResult> LikePostAsync([FromRoute] int postId, [FromRoute] int userId)
        {
            var result = await newsService.LikePostAsync(postId, userId);
            return Ok(result);
        }

        [HttpPost("{userId}/posts/{postId}/dislike")]
        public async Task<IActionResult> DislikePostAsync([FromRoute] int postId, [FromRoute] int userId)
        {
            var result = await newsService.DislikePostAsync(postId, userId);
            return Ok(result);
        }

        [HttpDelete("{userId}/posts/{postId}/rating")]
        public async Task<IActionResult> RemoveRatingAsync([FromRoute] int postId, [FromRoute] int userId)
        {
            var result = await newsService.RemoveRatingFromPostAsync(postId, userId);
            return Ok(result);
        }

        [HttpPost("{userId}/posts/{postId}/comments")]
        public async Task<IActionResult> SaveCommentAsync([FromRoute] int postId, [FromRoute] int userId, [FromBody] ContentRequest request)
        {
            var result = await newsService.SaveCommentAsync(postId, request.Content, userId);
            return Ok(result);
        }

        [HttpPut("{userId}/comments/{commentId}")]
        public async Task<IActionResult> UpdateCommentAsync([FromRoute] int commentId, [FromBody] ContentRequest request)
        {
            var result = await newsService.UpdateCommentAsync(commentId, request.Content);
            return Ok(result);
        }

        [HttpDelete("{userId}/comments/{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] int commentId)
        {
            var result = await newsService.DeleteCommentAsync(commentId);
            return Ok(result);
        }
    }

    public class FormatRequest
    {
        public string Text { get; set; }
    }

    public class ContentRequest
    {
        public string Content { get; set; }
    }
}
