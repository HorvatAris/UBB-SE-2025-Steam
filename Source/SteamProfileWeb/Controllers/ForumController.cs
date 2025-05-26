using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamProfileWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace SteamProfileWeb.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private readonly IForumService _forumService;
        private readonly IUserService _userService;
        private readonly IForumRepository _forumRepository;

        public ForumController(IForumService forumService, IUserService userService, IForumRepository forumRepository)
        {
            _forumService = forumService ?? throw new ArgumentNullException(nameof(forumService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _forumRepository = forumRepository ?? throw new ArgumentNullException(nameof(forumRepository));
        }

        public IActionResult Index(int page = 0,
                                  string sortOption = "recent",
                                  bool positiveScoreOnly = false,
                                  string searchFilter = null)
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
                totalPosts = _forumService.GetPostCount(positiveScoreOnly, null, searchFilter);

                // Get posts for current page
                posts = _forumService.GetPagedPosts((uint)page, pageSize, positiveScoreOnly, null, searchFilter);

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

                var allPosts = _forumService.GetTopPosts(filter);
                totalPosts = allPosts.Count;

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
                CurrentUserId = GetCurrentUserId(),
                HasNextPage = hasNextPage,
                HasPreviousPage = page > 0,
                TotalPosts = totalPosts
            };

            return View(viewModel);
        }

        public IActionResult PostDetail(int id)
        {
            // Get posts from forum service
            var posts = _forumService.GetPagedPosts(0, 1000, false, null, null);
            var post = posts.Find(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            // Get comments for the post
            var comments = _forumService.GetComments(id);

            // Create view model
            var viewModel = new PostDetailViewModel
            {
                Post = post,
                Comments = comments,
                CurrentUserId = GetCurrentUserId()
            };

            return View(viewModel);
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
        public IActionResult CreatePost(CreatePostViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Get current date in format expected by the service
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                // Get current user ID
                int currentUserId = GetCurrentUserId();

                // Create post using repository with explicit user ID
                _forumRepository.CreatePost(viewModel.Title, viewModel.Content, currentUserId, currentDate, viewModel.GameId);

                TempData["SuccessMessage"] = "Post created successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating post: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(AddCommentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(PostDetail), new { id = viewModel.PostId });
            }

            // Get current date in format expected by the service
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                // Get current user ID
                int currentUserId = GetCurrentUserId();

                // Create comment using repository with explicit user ID
                _forumRepository.CreateComment(viewModel.Content, viewModel.PostId, currentDate, currentUserId);

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
        public IActionResult VotePost(int postId, int voteValue)
        {
            try
            {
                // Get current user ID
                int currentUserId = GetCurrentUserId();

                // Vote on post using repository with explicit user ID
                _forumRepository.VoteOnPost(postId, voteValue, currentUserId);

                // Get updated post to return new score
                var posts = _forumService.GetPagedPosts(0, 1000, false, null, null);
                var post = posts.Find(p => p.Id == postId);
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
        public IActionResult VoteComment(int commentId, int voteValue)
        {
            try
            {
                // Get current user ID
                int currentUserId = GetCurrentUserId();

                // Vote on comment using repository with explicit user ID
                _forumRepository.VoteOnComment(commentId, voteValue, currentUserId);

                // Get updated comment to return new score (this might need adjustment based on your service interface)
                var comments = _forumService.GetComments(0); // Using 0 as post ID to get all comments
                var comment = comments.Find(c => c.Id == commentId);
                int updatedScore = comment?.Score ?? 0;

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
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int postId)
        {
            try
            {
                // Delete post
                _forumService.DeletePost(postId);

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
        public IActionResult DeleteComment(int commentId, int postId)
        {
            try
            {
                // Delete comment
                _forumService.DeleteComment(commentId);

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

        // Get current user ID from claims or authentication system
        private int GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                // First, try to get user from UserService
                try
                {
                    var currentUser = _userService.GetCurrentUser();
                    if (currentUser != null)
                    {
                        return (int)currentUser.UserId;
                    }
                }
                catch
                {
                    // Fall through to other methods if this fails
                }

                // Then, try to get from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                // Try to get from name claim
                var nameClaim = User.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                {
                    // Look up user by username
                    try
                    {
                        var user = _userService.GetUserByUsername(nameClaim.Value);
                        if (user != null)
                        {
                            return (int)user.UserId;
                        }
                    }
                    catch
                    {
                        // Continue to fallback
                    }
                }
            }

            // If we get here and can't determine user ID, log it for debugging
            Console.WriteLine("Warning: Unable to determine current user ID, defaulting to 1");
            return 1; // Default to user ID 1 if not authenticated
        }
    }
}