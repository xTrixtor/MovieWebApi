namespace MyAwesomeWebApi.Models.User
{
    public class UserResponseModel
    {
        public int UserID { get; set; }
        public string JWTToken { get; set; }
        public string Response { get; set; } = "";
    }
}
