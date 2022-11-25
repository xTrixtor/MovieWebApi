using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using MimeKit;
using MyAwesomeWebApi.Models.Mail;

namespace MyAwesomeWebApi.DataStore
{
    public class MailService
    {
        private readonly IConfiguration _configuration;
        private GmailSmtpModel _smtpModel;

        public MailService(IConfiguration configuration)
        {
            this._configuration = configuration;
            _smtpModel = new GmailSmtpModel();
        }
        public async Task SendEmailAsync(MailModel mail)
        {
            _configuration.GetSection("GmailSmtp").Bind(_smtpModel);
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_smtpModel.SmtpUsername));
            email.To.Add(MailboxAddress.Parse(mail.EmailTo));
            email.Subject = mail.Subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = mail.TextPart };


            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(_smtpModel.SmtpHost, _smtpModel.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpModel.SmtpUsername, _smtpModel.SmtpPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }

        public EmailResponseModel VerifyMailRequest(MailModel mail)
        {
            var mailProperties = typeof(MailModel).GetProperties();
            foreach (var prop in mailProperties)
            {
                var propValue = (string?)prop.GetValue(mail);
                if (String.IsNullOrEmpty(propValue))
                    return new EmailResponseModel { EmailProperty = prop.Name, IsValid = false };
            }
            return new EmailResponseModel { IsValid = true };
        }
    }
}
