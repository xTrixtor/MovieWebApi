using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAwesomeWebApi.DataStore;
using MyAwesomeWebApi.Models.Mail;
using MyAwesomeWebApi.Models.Reset;
using MyAwesomeWebApi.Models.User;
using System.Security.Cryptography;

namespace MyAwesomeWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetController : ControllerBase
    {
        private readonly ResetService _resetService;
        private readonly MailService _mailService;
        private readonly UserService _userService;


        public ResetController(ResetService resetService, MailService mailService, UserService userService)
        {
            this._resetService = resetService;
            this._mailService = mailService;
            this._userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Password([FromQuery] string username)
        {
            var dbUser = await _userService.SelectUserAsync(new User { Username = username});
            if (dbUser == null) { return BadRequest("Diesen User gibt es nicht"); }
            var resetCode = await _resetService.InsertResetCodeAsync(dbUser.id);
            var resetPasswordLink = $"http://127.0.0.1:5173/ResetPassword/{resetCode}";
            await _mailService.SendEmailAsync(new MailModel { EmailTo = "nicosteam857@gmail.com", Subject = "ForgottenEmail", TextPart = $"Bitte Klicken Sie auf diesen Link um Ihr Password zu reseten: {resetPasswordLink}" });
            return Ok("Es wurde eine Email verschickt! Bitte checke deine Emails");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateNewPassword([FromBody] CreateNewPasswordModel createNewPassword)
        {
            var dbUser = await _userService.SelectUserAsync(new User { Username = createNewPassword.Username});
            if (dbUser is null){ return BadRequest("Diesen User gibt es nicht"); }
            CreatePasswordHash(createNewPassword.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            var responseString = await _resetService.CreateNewPasswordAsync(dbUser.id,createNewPassword.ResetCode, passwordHash, passwordSalt);
            return Ok(responseString);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
