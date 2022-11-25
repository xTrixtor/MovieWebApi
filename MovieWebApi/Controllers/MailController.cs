using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAwesomeWebApi.DataStore;
using MyAwesomeWebApi.Models.Mail;
using System.Diagnostics;

namespace MyAwesomeWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly MailService _mailService;

        public MailController(MailService mailService)
        {
            this._mailService = mailService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SendMail([FromBody] MailModel mail)
        {
            var emailResponseModel = _mailService.VerifyMailRequest(mail);

            if (emailResponseModel is null)
                return BadRequest($"EmailResponse failed");
            if (!emailResponseModel.IsValid) 
                return BadRequest($"Fehlender Parameter: {emailResponseModel.EmailProperty}");

            try
            {
                await _mailService.SendEmailAsync(mail);
            }
            catch (Exception e)
            {
                return BadRequest($"Unbekannter Fehler {e.Message}");
            }
            return Ok("Email versand");
        }

        
    }
}
