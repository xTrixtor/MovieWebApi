namespace MyAwesomeWebApi.Models.Mail
{
    public class GmailSmtpModel
    {
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        public string? SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        
    }
}
