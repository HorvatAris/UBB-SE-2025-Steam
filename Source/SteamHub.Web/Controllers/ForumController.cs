using SteamHub.ApiContract.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private readonly IForumService _forumService;
        private readonly IUserService _userService;

        public ForumController(IForumService forumService, IUserService userService)
        {
            _forumService = forumService ?? throw new ArgumentNullException(nameof(forumService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<IActionResult> Index(int page = 0,
                                              string sortOption = "recent",
                                              bool positiveScoreOnly = false,
                                              string searchFilter = null)
        {
            try
            {
                // Default page size
                uint pageSize = 10;

                // Get posts based on sort option
                List<ForumPost> posts;
                int totalPosts = 0;
                bool hasNextPage = false;

                if (sortOption == "recent")
                {
                    // Get the total count of posts with current filters
                    totalPosts = await _forumService.GetPostCountAsync(positiveScoreOnly, null, searchFilter);

                    // Get posts for current page
                    posts = await _forumService.GetPagedPostsAsync((uint)page, pageSize, positiveScoreOnly, null, searchFilter);

                    // Check if there's a next page
                    hasNextPage = (page + 1) * pageSize < totalPosts;
                }
                else
                {
                    // Convert sort option to TimeSpanFilter
                    TimeSpanFilter filter = sortOption switch
                    {
                        "today" => TimeSpanFilter.Day,
                        "week" => TimeSpanFilter.Week,
                        "month" => TimeSpanFilter.Month,
                        "year" => TimeSpanFilter.Year,
                        "alltime" => TimeSpanFilter.AllTime,
                        _ => TimeSpanFilter.AllTime
                    };

                    var allPosts = await _forumService.GetTopPostsAsync(filter);
                    totalPosts = allPosts.Count(); // Fixed: use Count() method

                    // Apply pagination to top posts
                    posts = allPosts.Skip(page * (int)pageSize).Take((int)pageSize).ToList();
                    hasNextPage = (page + 1) * (int)pageSize < totalPosts;
                }

                // Create view model
                var viewModel = new ForumViewModel
                {
                    Posts = posts,
                    CurrentPage = page,
                    PositiveScoreOnly = positiveScoreOnly,
                    SortOption = sortOption,
                    SearchFilter = searchFilter,
                    CurrentUserId = await GetCurrentUserIdAsync(),
                    HasNextPage = hasNextPage,
                    HasPreviousPage = page > 0,
                    TotalPosts = totalPosts
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading forum: {ex.Message}";
                return View(new ForumViewModel { Posts = new List<ForumPost>() });
            }
        }

        public async Task<IActionResult> PostDetail(int id)
        {
            try
            {
                // Get posts from forum service
                var posts = await _forumService.GetPagedPostsAsync(0, 1000, false, null, null);
                var post = posts.FirstOrDefault(p => p.Id == id);

                if (post == null)
                {
                    return NotFound();
                }

                // Get comments for the post
                var comments = await _forumService.GetCommentsAsync(id);

                // Create view model
                var viewModel = new PostDetailViewModel
                {
                    Post = post,
                    Comments = comments,
                    CurrentUserId = await GetCurrentUserIdAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading post: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult CreatePost()
        {
            return View(new CreatePostViewModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(CreatePostViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                // Get current date in format expected by the service
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Create post using service
                await _forumService.CreatePostAsync(viewModel.Title, viewModel.Content, currentDate, viewModel.GameId);

                TempData["SuccessMessage"] = "Post created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating post: {ex.Message}";
                return View(viewModel);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(AddCommentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(PostDetail), new { id = viewModel.PostId });
            }

            try
            {
                // Get current date in format expected by the service
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Create comment using service
                await _forumService.CreateCommentAsync(viewModel.Content, viewModel.PostId, currentDate);

                TempData["SuccessMessage"] = "Comment added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error adding comment: {ex.Message}";
            }

            return RedirectToAction(nameof(PostDetail), new { id = viewModel.PostId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> VotePost(int postId, int voteValue)
        {
            try
            {
                // Vote on post using service
                await _forumService.VoteOnPostAsync(postId, voteValue);

                // Get updated post to return new score
                var posts = await _forumService.GetPagedPostsAsync(0, 1000, false, null, null);
                var post = posts.FirstOrDefault(p => p.Id == postId);
                int updatedScore = post?.Score ?? 0;

                // Return JSON result for AJAX updates with updated score
                return Json(new { success = true, score = updatedScore });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> VoteComment(int commentId, int voteValue)
        {
            try
            {
                // Vote on comment using service
                await _forumService.VoteOnCommentAsync(commentId, voteValue);

                // For now, return a success without the updated score
                return Json(new { success = true, score = 0 }); 
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int postId)
        {
            try
            {
                // Delete post
                await _forumService.DeletePostAsync(postId);

                // Redirect to forum index
                TempData["SuccessMessage"] = "Post deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // If deletion fails, show an error and return to the forum
                TempData["ErrorMessage"] = $"Failed to delete post: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId, int postId)
        {
            try
            {
                // Delete comment
                await _forumService.DeleteCommentAsync(commentId);

                // Redirect back to post detail
                TempData["SuccessMessage"] = "Comment deleted successfully!";
                return RedirectToAction(nameof(PostDetail), new { id = postId });
            }
            catch (Exception ex)
            {
                // If deletion fails, show an error and return to the post
                TempData["ErrorMessage"] = $"Failed to delete comment: {ex.Message}";
                return RedirectToAction(nameof(PostDetail), new { id = postId });
            }
        }

        // Private helper method using IUserService
        private async Task<int> GetCurrentUserIdAsync()
        {
            try
            {
                var currentUser = await _userService.GetCurrentUserAsync();
                return currentUser != null ? (int)currentUser.UserId : 0;
            }
            catch
            {
                return 0; // Default if user service fails
            }
        }
    }
}