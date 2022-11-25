using Microsoft.AspNetCore.Mvc;
using MyAwesomeWebApi.DataStore;
using MyAwesomeWebApi.Models.Movie;
using Microsoft.AspNetCore.Authorization;
using MyAwesomeWebApi;

namespace MyAwesomeWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly MovieService _movieService;
        private readonly JwtAuthenticationManager _jwtAuthenticationManager;

        public MovieController(MovieService movieService, JwtAuthenticationManager jwtAuthenticationManager)
        {
            this._movieService = movieService;
            _jwtAuthenticationManager = jwtAuthenticationManager;
        }

        

        [Authorize(Roles = "Guest")]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieService.GetMovies();
            return Ok(movies.Take(100));
        }
        [Authorize(Roles = "User")]
        [HttpGet("[action]")]
        public async Task<MovieDetailsModel> FindMovie([FromQuery] int id)
        {
            var movies = await _movieService.FindMovie(id);
            return movies;
        }
        [Authorize(Roles = "User")]
        [HttpGet("[action]")]
        public async Task<IEnumerable<MovieModel>> GetHighestVotedMovies()
        {
            return await _movieService.GetHighestVotedMoviesAsync();
        }
        [Authorize(Roles = "Guest,User")]
        [HttpGet("[action]")]
        public async Task<IEnumerable<MovieModel>> GetRandomMovies()
        {
            return await _movieService.GetRandomMoviesAsync();
        }
    }
}
