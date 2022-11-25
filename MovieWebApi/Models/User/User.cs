using System.Security.Permissions;

namespace MyAwesomeWebApi.Models.User
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
