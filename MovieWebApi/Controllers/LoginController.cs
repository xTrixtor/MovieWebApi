using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyAwesomeWebApi.DataStore;
using System.Security.Cryptography;
using MyAwesomeWebApi.Models.User;

namespace MyAwesomeWebApi.Controllers
{
    [ApiController]
    [Route("api/")]
    public class LoginController : ControllerBase
    {
        private readonly JwtAuthenticationManager _jwtAuthenticationManager;
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public LoginController(JwtAuthenticationManager jwtAuthenticationManager, AuthService authenticationService, UserService userService)
        {
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _authService = authenticationService;
            this._userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register([FromBody]User user)
        {
            var dbUser = await _userService.SelectUserAsync(user);
            if (dbUser != null)
                return BadRequest("Es gibt diesen User schon");

            CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
            await _userService.CreateUser(user.Username, passwordSalt, passwordHash);

            return Ok("You are registered now!");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<UserResponseModel>> Login([FromBody]User user)
        {
            var dbUser = await _authService.GetUserAsync(user);
            if (dbUser is null)
                return BadRequest("User not found");

            if (!VerifyPasswordHash(user.Password, dbUser.password_hash, dbUser.password_salt))
                return BadRequest("Wrong password");

            var jwtToken = _jwtAuthenticationManager.Authenticate(dbUser);

            return Ok(new UserResponseModel { JWTToken = jwtToken, UserID = dbUser.id, Response = "Du bist eingeloggt!"});
        }

        [AllowAnonymous]
        [HttpGet("TestConnection")]
        public async Task<ActionResult<UserResponseModel>> TestConnection()
        {
            var result = _authService.TestConnection();
            return Ok($"Test: {result}" );
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }
}
