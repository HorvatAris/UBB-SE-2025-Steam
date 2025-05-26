using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Data;
using BusinessLayer.DataContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Repositories
{
    public class ForumRepository : IForumRepository
    {
        private readonly ApplicationDbContext context;

        public ForumRepository(ApplicationDbContext newContext)
        {
            context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        private string TimeSpanFilterToString(TimeSpanFilter filter)
        {
            switch (filter)
            {
                case TimeSpanFilter.Day:
                    return "DAY";
                case TimeSpanFilter.Week:
                    return "WEEK";
                case TimeSpanFilter.Month:
                    return "MONTH";
                case TimeSpanFilter.Year:
                    return "YEAR";
                default:
                    return string.Empty;
            }
        }

        public List<ForumPost> GetTopPosts(TimeSpanFilter filter)
        {
            var posts = context.ForumPosts.AsNoTracking();
            if (filter != TimeSpanFilter.AllTime)
            {
                var span = filter switch
                {
                    TimeSpanFilter.Day => TimeSpan.FromDays(1),
                    TimeSpanFilter.Week => TimeSpan.FromDays(7),
                    TimeSpanFilter.Month => TimeSpan.FromDays(30),
                    TimeSpanFilter.Year => TimeSpan.FromDays(365),
                    _ => TimeSpan.MaxValue
                };
                var cutoff = DateTime.Now.Subtract(span);
                posts = posts.Where(p => p.TimeStamp >= cutoff);
            }

            return posts.OrderByDescending(p => p.Score)
                .Take(20)
                .ToList();
        }

        public void CreatePost(string title, string body, int authorId, string date, int? gameId)
        {
            var post = new ForumPost
            {
                Title = title,
                Body = body,
                Score = 0,
                TimeStamp = DateTime.Parse(date),
                AuthorId = (int)authorId,
                GameId = gameId
            };
            context.ForumPosts.Add(post);
            context.SaveChanges();
        }

        public void DeletePost(int postId)
        {
            var post = new ForumPost { Id = postId };
            if (post != null)
            {
                context.ForumPosts.Remove(post);
                context.SaveChanges();
            }
        }

        public void CreateComment(string body, int postId, string date, int authorId)
        {
            var comment = new ForumComment
            {
                Body = body,
                Score = 0,
                TimeStamp = DateTime.Parse(date),
                AuthorId = authorId,
                PostId = postId
            };

            context.ForumComments.Add(comment);
            context.SaveChanges();
        }

        public void DeleteComment(int commentId)
        {
            var comment = new ForumComment { Id = commentId };
            if (comment != null)
            {
                context.ForumComments.Remove(comment);
                context.SaveChanges();
            }
        }

        private int GetScore<T>(DbSet<T> dbSet, object key)
            where T : class
        {
            var entity = dbSet.Find(key) as dynamic;
            return entity != null ? (int)entity.Score : 0;
        }

        public void VoteOnPost(int postId, int voteValue, int userId)
        {
            var post = context.ForumPosts.Find((int)postId);
            if (post == null)
            {
                return;
            }
            // Determine existing votes
            var liked = context.UserLikedPosts.Find(userId, (int)postId);
            var disliked = context.UserDislikedPosts.Find(userId, (int)postId);

            if (liked == null && disliked == null)
            {
                // first vote
                post.Score += voteValue;
                if (voteValue > 0)
                {
                    context.UserLikedPosts.Add(new UserLikedPost { UserId = userId, PostId = postId });
                }
                else
                {
                    context.UserDislikedPosts.Add(new UserDislikedPost { UserId = userId, PostId = postId });
                }
            }
            else if (liked != null)
            {
                if (voteValue > 0)
                {
                    // retract like
                    post.Score -= voteValue;
                    context.UserLikedPosts.Remove(liked);
                }
                else
                {
                    // switch to dislike
                    post.Score += 2 * voteValue; // subtract like and add dislike
                    context.UserLikedPosts.Remove(liked);
                    context.UserDislikedPosts.Add(new UserDislikedPost { UserId = userId, PostId = postId });
                }
            }
            else if (disliked != null)
            {
                if (voteValue < 0)
                {
                    // retract dislike
                    post.Score -= voteValue;
                    context.UserDislikedPosts.Remove(disliked);
                }
                else
                {
                    // switch to like
                    post.Score += 2 * voteValue; // remove dislike then add like
                    context.UserDislikedPosts.Remove(disliked);
                    context.UserLikedPosts.Add(new UserLikedPost { UserId = userId, PostId = postId });
                }
            }
            context.SaveChanges();
        }

        public void VoteOnComment(int commentId, int voteValue, int userId)
        {
            var comment = context.ForumComments.Find((int)commentId);
            if (comment == null)
            {
                return;
            }

            var liked = context.UserLikedComments.Find(userId, (int)commentId);
            var disliked = context.UserDislikedComments.Find(userId, (int)commentId);

            if (liked == null && disliked == null)
            {
                comment.Score += voteValue;
                if (voteValue > 0)
                {
                    context.UserLikedComments.Add(new UserLikedComment { UserId = userId, CommentId = commentId });
                }
                else
                {
                    context.UserDislikedComments.Add(new UserDislikedComment { UserId = userId, CommentId = commentId });
                }
            }
            else if (liked != null)
            {
                if (voteValue > 0)
                {
                    comment.Score -= voteValue;
                    context.UserLikedComments.Remove(liked);
                }
                else
                {
                    comment.Score -= 2 * voteValue;
                    context.UserLikedComments.Remove(liked);
                    context.UserDislikedComments.Add(new UserDislikedComment { UserId = userId, CommentId = commentId });
                }
            }
            else if (disliked != null)
            {
                if (voteValue < 0)
                {
                    comment.Score -= voteValue;
                    context.UserDislikedComments.Remove(disliked);
                }
                else
                {
                    comment.Score += 2 * voteValue;
                    context.UserDislikedComments.Remove(disliked);
                    context.UserLikedComments.Add(new UserLikedComment { UserId = userId, CommentId = commentId });
                }
            }

            context.SaveChanges();
        }

#nullable enable
        public List<ForumPost> GetPagedPosts(uint pageNumber, uint pageSize, bool positiveScoreOnly = false, int? gameId = null, string? filter = null)
        {
            var query = context.ForumPosts.AsQueryable();

            if (gameId.HasValue)
            {
                query = query.Where(p => p.GameId == gameId.Value);
            }
            if (positiveScoreOnly)
            {
                query = query.Where(p => p.Score >= 0);
            }
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(p => p.Title.Contains(filter) || p.Body.Contains(filter));
            }

            return query
                .OrderByDescending(p => p.TimeStamp)
                .Skip((int)(pageNumber * pageSize))
                .Take((int)pageSize)
                .ToList();
        }

        public int GetPostCount(bool positiveScoreOnly = false, int? gameId = null, string? filter = null)
        {
            var query = context.ForumPosts.AsQueryable();

            if (gameId.HasValue)
            {
                query = query.Where(p => p.GameId == gameId.Value);
            }
            if (positiveScoreOnly)
            {
                query = query.Where(p => p.Score >= 0);
            }
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(p => p.Title.Contains(filter) || p.Body.Contains(filter));
            }

            return query.Count();
        }

        public List<ForumComment> GetComments(int postId)
        {
            return context.ForumComments
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.TimeStamp)
                .ToList();
        }
    }
}