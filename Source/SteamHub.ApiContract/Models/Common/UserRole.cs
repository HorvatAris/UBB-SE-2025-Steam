using System.Text.Json.Serialization;

namespace SteamHub.ApiContract.Models.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        /// <summary>
        /// Regular user with standard permissions.
        /// </summary>
        User = 0,
        
        /// <summary>
        /// Developer with additional permissions for game management.
        /// </summary>
        Developer = 1
    }
} 