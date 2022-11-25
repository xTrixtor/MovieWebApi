namespace MyAwesomeWebApi.Models.Mail
{
    public class MailModel
    {
        public string? EmailTo { get; set; }
        public string? Subject { get; set; } = string.Empty;
        public string? TextPart { get; set; } = string.Empty;
    }
}
