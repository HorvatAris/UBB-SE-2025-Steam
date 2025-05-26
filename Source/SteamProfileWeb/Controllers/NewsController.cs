using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using SteamProfileWeb.ViewModels;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace SteamProfileWeb.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly INewsService newsService;
        private readonly IUserService userService;

        public NewsController(INewsService newsService, IUserService userService)
        {
            this.newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // Get News
        public IActionResult Index(int page = 1, string search = "")
        {
            // Not calling GetCurrentUserId because we want to redirect them to login first
            string userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth"); //  redirect to login
            }

            var viewModel = new NewsViewModel
            {
                CurrentPage = page,
                SearchText = search,
                IsDeveloper = userService.GetUserByIdentifier(userId).IsDeveloper
            };

            viewModel.Posts = newsService.LoadNextPosts(page, search);

            // Calculate total pages
            int totalPosts = viewModel.Posts.Count;
            viewModel.TotalPages = (int)Math.Ceiling((double)totalPosts / NewsViewModel.PageSize);

            ViewData["UserService"] = userService;

            return View(viewModel);
        }

        // Get News/Details
        public IActionResult Details(int id)
        {
            var post = newsService.LoadNextPosts(1, "").FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var author = userService.GetUserByIdentifier(post.AuthorId);
            var currentUserId = GetCurrentUserId();
            var currentUser = userService.GetUserByIdentifier(currentUserId);

            var viewModel = new NewsPostViewModel
            {
                Post = post,
                Author = author,
                IsCurrentUserAuthor = post.AuthorId == currentUser.UserId || currentUser.IsDeveloper
            };

            // Load Comments
            List<Comment> comments = newsService.LoadNextComments(id);
            foreach (var comment in comments)
            {
                var commentAuthor = userService.GetUserByIdentifier(comment.AuthorId);
                viewModel.Comments.Add(new CommentViewModel
                {
                    Comment = comment,
                    Author = commentAuthor,
                    IsCurrentUserAuthor = comment.AuthorId == currentUser.UserId || currentUser.IsDeveloper
                });
            }

            return View(viewModel);
        }

        // Get News/Create
        [Authorize]
        public IActionResult Create()
        {
            var currentUserId = GetCurrentUserId();
            // Check if user is a developer
            if (!userService.GetUserByIdentifier(currentUserId).IsDeveloper)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("CreateEdit", new CreateNewsPostViewModel());
        }

        // Post News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(CreateNewsPostViewModel model)
        {
            var currentUserId = GetCurrentUserId();
            // Check if user is a developer
            if (!userService.GetUserByIdentifier(currentUserId).IsDeveloper)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                string formattedContent = newsService.FormatAsPost(model.Content);
                bool success = newsService.SavePost(formattedContent);

                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View("CreateEdit", model);
        }

        // Get News/Edit
        [Authorize]
        public IActionResult Edit(int id)
        {
            var currentUserId = GetCurrentUserId();
            // Check if user is a developer
            if (!userService.GetUserByIdentifier(currentUserId).IsDeveloper)
            {
                return RedirectToAction(nameof(Index));
            }

            var post = newsService.LoadNextPosts(1, "").FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            // Extract content from HTML
            string content = post.Content;
            int startIndex = content.IndexOf("<body>") + "<body>".Length;
            int endIndex = content.IndexOf("</body>");

            if (startIndex >= 0 && endIndex > startIndex)
            {
                content = content.Substring(startIndex, endIndex - startIndex);
            }

            var viewModel = new CreateNewsPostViewModel
            {
                PostId = id,
                Content = content
            };

            return View("CreateEdit", viewModel);
        }

        // Post News/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Edit(int id, CreateNewsPostViewModel model)
        {
            var currentUserId = GetCurrentUserId();
            // Check if user is a developer
            if (!userService.GetUserByIdentifier(currentUserId).IsDeveloper)
            {
                return RedirectToAction(nameof(Index));
            }

            if (id != model.PostId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                string formattedContent = newsService.FormatAsPost(model.Content);
                bool success = newsService.UpdatePost(id, formattedContent);
                if (success)
                {
                    return RedirectToAction(nameof(Details), new { id });
                }
            }

            return View("CreateEdit", model);
        }

        // Post News/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult DeleteConfirmed(int id)
        {
            var currentUserId = GetCurrentUserId();
            // Check if user is a developer
            if (!userService.GetUserByIdentifier(currentUserId).IsDeveloper)
            {
                return RedirectToAction(nameof(Index));
            }

            bool success = newsService.DeletePost(id);
            return RedirectToAction(nameof(Index));
        }

        // Post News/Like
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Like(int id, string returnUrl = null)
        {
            // Get the post to check current rating state
            var posts = newsService.LoadNextPosts(1, "");
            var post = posts.FirstOrDefault(p => p.Id == id);
            if (post != null)
            {
                if (post.ActiveUserRating == true)
                {
                    // User already liked - remove rating
                    newsService.RemoveRatingFromPost(id);
                }
                else if (post.ActiveUserRating == false)
                {
                    // User previously disliked - remove and add like
                    newsService.RemoveRatingFromPost(id);
                    newsService.LikePost(id);
                }
                else
                {
                    // No previous rating - add like
                    newsService.LikePost(id);
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // Post News/Dislike
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Dislike(int id, string returnUrl = null)
        {
            // Get the post to check the current rating state
            var posts = newsService.LoadNextPosts(1, "");
            var post =  posts.FirstOrDefault(p => p.Id == id);

            if (post != null)
            {
                if (post.ActiveUserRating == false)
                {
                    // User already disliked - remove rating
                    newsService.RemoveRatingFromPost(id);
                }
                else if (post.ActiveUserRating == true)
                {
                    // User previously liked - remove and add dislike
                    newsService.RemoveRatingFromPost(id);
                    newsService.DislikePost(id);
                }
                else
                {
                    // No previous rating -- add dislike
                    newsService.DislikePost(id);
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // Post News/RemoveRating
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveRating(int id, string returnUrl = null)
        {
            bool success = newsService.RemoveRatingFromPost(id);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // Post News/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int postId, string content, string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(content))
            {
                string formattedContent = newsService.FormatAsPost(content);
                bool success = newsService.SaveComment(postId, formattedContent);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Details), new { id = postId });
        }

        // Post News/EditComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditComment(int commentId, string content, int postId, string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(content))
            {
                string formattedContent = newsService.FormatAsPost(content);
                bool success = newsService.UpdateComment(commentId, formattedContent);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Details), new { id = postId });
        }

        // Post News/DeleteComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteComment(int commentId, int postId, string returnUrl = null)
        {
            bool success = newsService.DeleteComment(commentId);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Details), new { id = postId });
        }

        // Post News/FormatPost
        [HttpPost]
        public IActionResult FormatPost(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return Content("<din class='alert alert-info'>No content to preview</div>");
            }

            string formattedContent = newsService.FormatAsPost(content);
            return Content(formattedContent);
        }

        // !! When the user system is setup correctly uncomment the commented section !! //
        private int GetCurrentUserId()
        {

            /*
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var currentUser = userService.GetCurrentUser();
                    if (currentUser != null)
                    {
                        return (int)currentUser.UserId;
                    }
                }
                catch
                {
                    // Fall through to other methods if this fails
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            */
            return 1; // Default to user ID 1 if not authenticated
        }
    }
}
