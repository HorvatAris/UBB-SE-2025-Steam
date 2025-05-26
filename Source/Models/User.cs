using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        // This property is used for input/output but never stored in the database
        // The actual password is stored as a hash in the database
        public string Password { get; set; }
        public bool IsDeveloper { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        public void UpdateFrom(User other)
        {
            Email = other.Email;
            Username = other.Username;
            Password = other.Password;
            IsDeveloper = other.IsDeveloper;
            CreatedAt = other.CreatedAt;
            LastLogin = other.LastLogin;
        }
    }

    public enum FriendshipStatus
    {
        NotFriends,
        Friends,
        RequestSent,
        RequestReceived
    }
}
