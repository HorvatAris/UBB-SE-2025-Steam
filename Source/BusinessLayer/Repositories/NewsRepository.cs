using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Data;
using BusinessLayer.Models;
using BusinessLayer.DataContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly ApplicationDbContext context;
        public const int PAGE_SIZE = 9;

        public NewsRepository(ApplicationDbContext newContext) // Constructor has parameter for injection
        {
            context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        /// <summary>
        /// Update a specific post's like count in the database
        /// </summary>
        /// <param name="postId">Post that has to be updated given by its id</param>
        /// <returns>The result of the update (successful or not)</returns>
        /// <exception cref="Exception">Throw an error if the connection or the query execution failed</exception>
        public int UpdatePostLikeCount(int postId)
        {
            var post = context.NewsPosts.Find(postId);
            if (post == null)
            {
                return 0;
            }
            post.NrLikes++;
            return context.SaveChanges();
        }

        /// <summary>
        /// Update a specific post's dislike count in the database
        /// </summary>
        /// <param name="postId">Post that has to be updated given by its id</param>
        /// <returns>The result of the update (successful or not)</returns>
        /// <exception cref="Exception">Throw an error if the connection or the query execution failed</exception>
        public int UpdatePostDislikeCount(int postId)
        {
            var post = context.NewsPosts.Find(postId);
            if (post == null)
            {
                return 0;
            }

            post.NrDislikes++;
            return context.SaveChanges();
        }

        /// <summary>
        /// Save a new rating into the database by a user targeting the given post
        /// </summary>
        /// <param name="postId">Target posts id</param>
        /// <param name="userId">User's id that left the rating</param>
        /// <param name="ratingType">Type of the rating (negative/positive)</param>
        /// <returns>The result of the query execution</returns>
        /// <exception cref="Exception">Throw an error if the connection or the execution failed</exception>
        public int AddRatingToPost(int postId, int userId, int ratingType)
        {
            var rating = new PostRatingType
            {
                PostId = postId,
                AuthorId = userId,
                RatingType = ratingType == 1 ? true : false
            };
            context.NewsPostRatingTypes.Add(rating);
            return context.SaveChanges();
        }

        /// <summary>
        /// Delete the target post's rating from the database
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <param name="userId">Author of the rating</param>
        /// <returns>The result of the execution</returns>
        /// <exception cref="Exception">Throw an error if the connection or the execution failed</exception>
        public int RemoveRatingFromPost(int postId, int userId)
        {
            var rating = context.NewsPostRatingTypes.Find(postId, userId);
            if (rating == null)
            {
                return 0;
            }

            var post = context.NewsPosts.Find(postId);
            if (post == null)
            {
                return 0;
            }
            if (rating.RatingType == false)
            {
                post.NrDislikes--;
            }
            else
            {
                post.NrLikes--;
            }

            context.NewsPostRatingTypes.Remove(rating);
            return context.SaveChanges();
        }

        /// <summary>
        /// Add a new comment to the target post and insert it in the database
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <param name="commentContent">Contents of the comment</param>
        /// <param name="userId">Author of the comment</param>
        /// <param name="commentDate">Date the comment is published</param>
        /// <returns>The result of the execution</returns>
        /// <exception cref="Exception">Throw an error if the connection or the execution failed</exception>
        public int AddCommentToPost(int postId, string commentContent, int userId, DateTime commentDate)
        {
            var comment = new Comment
            {
                PostId = postId,
                AuthorId = userId,
                Content = commentContent,
                CommentDate = commentDate,
                NrLikes = 0,
                NrDislikes = 0
            };

            // Update the comment count of the post
            var post = context.NewsPosts.Find(postId);
            if (post == null)
            {
                return 0;
            }
            post.NrComments++;

            context.NewsComments.Add(comment);
            return context.SaveChanges();
        }

        /// <summary>
        /// Update a target comment in the database
        /// </summary>
        /// <param name="commentId">Target comment</param>
        /// <param name="commentContent">New contents of the comment</param>
        /// <returns>The query execution result</returns>
        /// <exception cref="Exception">Throw an error if the connection or execution failed</exception>
        public int UpdateComment(int commentId, string commentContent)
        {
            var comment = context.NewsComments.Find(commentId);
            if (comment == null)
            {
                return 0;
            }

            comment.Content = commentContent;
            return context.SaveChanges();
        }

        /// <summary>
        /// Delete a specific comment from the database
        /// </summary>
        /// <param name="commentId">Target comment</param>
        /// <returns>The result of the execution</returns>
        /// <exception cref="Exception">Throw an error if the connection or the execution failed</exception>
        public int DeleteCommentFromDatabase(int commentId)
        {
            var comment = context.NewsComments.Find(commentId);
            if (comment == null)
            {
                return 0;
            }

            // Update the comment count of the post
            var post = context.NewsPosts.Find(comment.PostId);
            if (post != null)
            {
                post.NrComments--;
            }

            context.NewsComments.Remove(comment);
            return context.SaveChanges();
        }

        /// <summary>
        /// Search for all the comments in the database that has the given target post
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <returns>Read comments in a list</returns>
        /// <exception cref="Exception">Throw an error if anything failed</exception>
        public List<Comment> LoadFollowingComments(int postId)
        {
            return context.NewsComments
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CommentDate)
                .ToList();
        }

        /// <summary>
        /// Insert a post into the database
        /// </summary>
        /// <param name="userId">Author of the post</param>
        /// <param name="postContent">Contents of the post</param>
        /// <param name="postDate">Date the post is published</param>
        /// <returns>The result of the execution</returns>
        /// <exception cref="Exception">Throw an error if the connection or the execution failed</exception>
        public int AddPostToDatabase(int userId, string postContent, DateTime postDate)
        {
            var post = new Post
            {
                AuthorId = userId,
                Content = postContent,
                UploadDate = postDate,
                NrLikes = 0,
                NrDislikes = 0,
                NrComments = 0
            };
            context.NewsPosts.Add(post);
            return context.SaveChanges();
        }

        /// <summary>
        /// Update a specific post in the database
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <param name="postContent">New contents of the post</param>
        /// <returns>The result of the execution</returns>
        /// <exception cref="Exception">Throw an error if the connection or execution failed</exception>
        public int UpdatePost(int postId, string postContent)
        {
           var post = context.NewsPosts.Find(postId);
            if (post == null)
            {
                return 0;
            }
            post.Content = postContent;
            return context.SaveChanges();
        }

        /// <summary>
        /// Delete a post from the database
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <returns>The result of the query execution</returns>
        /// <exception cref="Exception">Throw an error if the connection or execution failed</exception>
        public int DeletePostFromDatabase(int postId)
        {
            var post = context.NewsPosts.Find(postId);
            if (post == null)
            {
                return 0;
            }
            context.NewsPosts.Remove(post);
            return context.SaveChanges();
        }

        /// <summary>
        /// Search for all the posts in the database matching the searched text
        /// Load all the ratings the current user has on any of the found posts
        /// </summary>
        /// <param name="pageNumber">Page number to calculate page offset</param>
        /// <param name="userId">User id to select ratings related to user</param>
        /// <param name="searchedText">Searched text</param>
        /// <returns>Posts matching the searched text as a list</returns>
        /// <exception cref="Exception">Throw an error if anything failed</exception>
        public List<Post> LoadFollowingPosts(int pageNumber, int userId, string searchedText)
        {
            // Ensure pageNumber is at least 1
            pageNumber = Math.Max(1, pageNumber);

            var query = context.NewsPosts
                .Where(p => EF.Functions.Like(p.Content, $"%{searchedText}%"))
                .OrderByDescending(p => p.UploadDate)
                .ThenByDescending(p => p.Id)
                .Skip((pageNumber - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToList();

            // load user's rating
            foreach (var post in query)
            {
                var rating = context.NewsPostRatingTypes.Find(post.Id, userId);
                post.ActiveUserRating = rating != null ? rating.RatingType : null;
            }
            return query;
        }
    }
}
