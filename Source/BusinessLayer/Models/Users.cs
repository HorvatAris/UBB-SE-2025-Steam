using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Created some users for initial showcase. When the server is setup this section may be removed.
namespace BusinessLayer.Models
{
    public class Users
    {
        private static Users? instance;
        private static List<User> users = new();
        public static Users Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Users();
                }

                return instance;
            }
        }

        private Users()
        {
            // ID  Username IsDeveloper
            users.Add(new(1, "JaneSmith", true));
            users.Add(new(2, "Iraphahell", false));
            users.Add(new(3, "XSlayder", false));
            users.Add(new(4, "Tristopher", true));
            users.Add(new(5, "Gumball", false));
        }

        /// <summary>
        /// Get the user instance by searching for its id
        /// </summary>
        /// <param name="id">id to search for</param>
        /// <returns>User instance that has the correct id</returns>
        /// <exception cref="Exception">Throw error if the user was not found by the given id</exception>
        public User? GetUserById(int id)
        {
            try
            {
                return users.Find(user => user.UserId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: user not found: " + ex.Message);
            }
        }
    }
}
