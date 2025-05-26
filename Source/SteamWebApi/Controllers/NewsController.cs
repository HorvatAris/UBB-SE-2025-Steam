using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SteamWebApi.Controllers
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

        [HttpGet("posts")]
        public IActionResult GetPosts([FromQuery] int page = 0, [FromQuery] string search = "")
        {
            var posts = newsService.LoadNextPosts(page, search);
            return Ok(posts);
        }

        [HttpGet("posts/{postId}/comments")]
        public IActionResult GetComments(int postId)
        {
            var comments = newsService.LoadNextComments(postId);
            return Ok(comments);
        }

        [HttpPost("format")]
        public IActionResult FormatAsPost([FromBody] FormatRequest request)
        {
            var formattedText = newsService.FormatAsPost(request.Text);
            return Ok(formattedText);
        }

        [HttpPost("posts")]
        public IActionResult SavePost([FromBody] ContentRequest request)
        {
            var result = newsService.SavePost(request.Content);
            return Ok(result);
        }

        [HttpPut("posts/{postId}")]
        public IActionResult UpdatePost(int postId, [FromBody] ContentRequest request)
        {
            var result = newsService.UpdatePost(postId, request.Content);
            return Ok(result);
        }

        [HttpDelete("posts/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            var result = newsService.DeletePost(postId);
            return Ok(result);
        }

        [HttpPost("posts/{postId}/like")]
        public IActionResult LikePost(int postId)
        {
            var result = newsService.LikePost(postId);
            return Ok(result);
        }

        [HttpPost("posts/{postId}/dislike")]
        public IActionResult DislikePost(int postId)
        {
            var result = newsService.DislikePost(postId);
            return Ok(result);
        }

        [HttpDelete("posts/{postId}/rating")]
        public IActionResult RemoveRating(int postId)
        {
            var result = newsService.RemoveRatingFromPost(postId);
            return Ok(result);
        }

        [HttpPost("posts/{postId}/comments")]
        public IActionResult SaveComment(int postId, [FromBody] ContentRequest request)
        {
            var result = newsService.SaveComment(postId, request.Content);
            return Ok(result);
        }

        [HttpPut("comments/{commentId}")]
        public IActionResult UpdateComment(int commentId, [FromBody] ContentRequest request)
        {
            var result = newsService.UpdateComment(commentId, request.Content);
            return Ok(result);
        }

        [HttpDelete("comments/{commentId}")]
        public IActionResult DeleteComment(int commentId)
        {
            var result = newsService.DeleteComment(commentId);
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