using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.User;
using SteamHub.Web.ViewModels;
using SteamHub.ApiContract.Models.Common;
using Microsoft.CodeAnalysis.CSharp;

namespace SteamHub.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly INewsService newsService;
        private readonly IUserDetails userDetails;
        private readonly IUserService userService;


        public NewsController(INewsService newsService, IUserService userService, IUserDetails userDetails)
        {
            this.newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.userDetails = userDetails ?? throw new ArgumentNullException(nameof(userDetails));
        }

        // Get News
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            // Not calling GetCurrentUserId because we want to redirect them to login first
            string userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth"); //  redirect to login
            }

            var posts = await newsService.LoadNextPostsAsync(page, search, userDetails.UserId);
            Console.Error.WriteLine($"Loaded {posts.Count} posts");
            // Fetch authors
            var authorsDict = new Dictionary<int, User>();
            foreach (var post in posts)
            {
                Console.Error.WriteLine($"AUTHOR ID {post.AuthorId}");
                var author = await userService.GetUserByIdentifierAsync(post.AuthorId);
                authorsDict[post.AuthorId] = author;
            }

            var viewModel = new NewsViewModel
            {
                CurrentPage = page,
                SearchText = search,
                IsDeveloper = userDetails.UserRole == UserRole.Developer,
                Posts = posts,
                Authors = authorsDict,
            };

            // Calculate total pages
            int totalPosts = viewModel.Posts.Count;
            viewModel.TotalPages = (int)Math.Ceiling((double)totalPosts / NewsViewModel.PageSize);

            ViewData["UserService"] = userDetails;

            return View(viewModel);
        }

        // Get News/Details
        public async Task<IActionResult> Details(int id)
        {
            var post = (await newsService.LoadNextPostsAsync(1, "", userDetails.UserId)).FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var author = await userService.GetUserByIdentifierAsync(post.AuthorId);
            var currentUser = userDetails;

            var viewModel = new NewsPostViewModel
            {
                Post = post,
                Author = author,
                IsCurrentUserAuthor = post.AuthorId == currentUser.UserId || currentUser.UserRole == UserRole.Developer,
            };

            // Load Comments
            List<Comment> comments = await newsService.LoadNextCommentsAsync(id, userDetails.UserId);
            foreach (var comment in comments)
            {
                var commentAuthor = await userService.GetUserByIdentifierAsync(comment.AuthorId);
                viewModel.Comments.Add(new CommentViewModel
                {
                    Comment = comment,
                    Author = commentAuthor,
                    IsCurrentUserAuthor = comment.AuthorId == currentUser.UserId || currentUser.UserRole == UserRole.Developer
                });
            }

            return View(viewModel);
        }

        // Get News/Create
        [Authorize]
        public IActionResult Create()
        {
            var currentUser = userDetails;
            // Check if user is a developer
            if (currentUser.UserRole != UserRole.Developer)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("CreateEdit", new CreateNewsPostViewModel());
        }

        // Post News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateNewsPostViewModel model)
        {
            var currentUser = userDetails;
            // Check if user is a developer
            if (currentUser.UserRole != UserRole.Developer)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                string formattedContent = await newsService.FormatAsPostAsync(model.Content);
                bool success = await newsService.SavePostAsync(formattedContent, userDetails.UserId);

                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View("CreateEdit", model);
        }

        // Get News/Edit
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = userDetails;
            // Check if user is a developer
            if (currentUser.UserRole != UserRole.Developer)
            {
                return RedirectToAction(nameof(Index));
            }

            var post = (await newsService.LoadNextPostsAsync(1, "", userDetails.UserId)).FirstOrDefault(p => p.Id == id);
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
        public async Task<IActionResult> Edit(int id, CreateNewsPostViewModel model)
        {
            var currentUser = userDetails;
            // Check if user is a developer
            if (currentUser.UserRole != UserRole.Developer)
            {
                return RedirectToAction(nameof(Index));
            }

            if (id != model.PostId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                string formattedContent = await newsService.FormatAsPostAsync(model.Content);
                bool success = await newsService.UpdatePostAsync(id, formattedContent, userDetails.UserId);
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = userDetails;
            // Check if user is a developer
            if (currentUser.UserRole != UserRole.Developer)
            {
                return RedirectToAction(nameof(Index));
            }

            bool success = await newsService.DeletePostAsync(id, userDetails.UserId);
            return RedirectToAction(nameof(Index));
        }

        // Post News/Like
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(int id, string returnUrl = null)
        {
            // Get the post to check current rating state
            var posts = await newsService.LoadNextPostsAsync(1, "", userDetails.UserId);
            var post = posts.FirstOrDefault(p => p.Id == id);
            if (post != null)
            {
                if (post.ActiveUserRating == true)
                {
                    // User already liked - remove rating
                    await newsService.RemoveRatingFromPostAsync(id, userDetails.UserId);
                }
                else if (post.ActiveUserRating == false)
                {
                    // User previously disliked - remove and add like
                    await newsService.RemoveRatingFromPostAsync(id, userDetails.UserId);
                    await newsService.LikePostAsync(id, userDetails.UserId);
                }
                else
                {
                    // No previous rating - add like
                    await newsService.LikePostAsync(id, userDetails.UserId);
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
        public async Task<IActionResult> Dislike(int id, string returnUrl = null)
        {
            // Get the post to check the current rating state
            var posts = await newsService.LoadNextPostsAsync(1, "", userDetails.UserId);
            var post = posts.FirstOrDefault(p => p.Id == id);

            if (post != null)
            {
                if (post.ActiveUserRating == false)
                {
                    // User already disliked - remove rating
                    await newsService.RemoveRatingFromPostAsync(id, userDetails.UserId);
                }
                else if (post.ActiveUserRating == true)
                {
                    // User previously liked - remove and add dislike
                    await newsService.RemoveRatingFromPostAsync(id, userDetails.UserId);
                    await newsService.DislikePostAsync(id, userDetails.UserId);
                }
                else
                {
                    // No previous rating -- add dislike
                    await newsService.DislikePostAsync(id, userDetails.UserId);
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
        public async Task<IActionResult> RemoveRating(int id, string returnUrl = null)
        {
            bool success = await newsService.RemoveRatingFromPostAsync(id, userDetails.UserId);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // Post News/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string content, string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(content))
            {
                string formattedContent = await newsService.FormatAsPostAsync(content);
                bool success = await newsService.SaveCommentAsync(postId, formattedContent, userDetails.UserId);
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
        public async Task<IActionResult> EditComment(int commentId, string content, int postId, string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(content))
            {
                string formattedContent = await newsService.FormatAsPostAsync(content);
                bool success = await newsService.UpdateCommentAsync(commentId, formattedContent, userDetails.UserId);
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
        public async Task<IActionResult> DeleteComment(int commentId, int postId, string returnUrl = null)
        {
            bool success = await newsService.DeleteCommentAsync(commentId, userDetails.UserId);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Details), new { id = postId });
        }

        // Post News/FormatPost
        [HttpPost]
        public async Task<IActionResult> FormatPost(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return Content("<din class='alert alert-info'>No content to preview</div>");
            }

            string formattedContent = await newsService.FormatAsPostAsync(content);
            return Content(formattedContent);
        }
    }
}
