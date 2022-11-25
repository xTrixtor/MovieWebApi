namespace MyAwesomeWebApi.Models.Reset
{
    public class CreateNewPasswordModel
    {
        public string ResetCode { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
