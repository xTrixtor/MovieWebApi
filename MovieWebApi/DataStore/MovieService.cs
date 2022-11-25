using Dapper;
using MyAwesomeWebApi.Models.Movie;
using MySql.Data.MySqlClient;
using System.Data;

namespace MyAwesomeWebApi.DataStore
{
    public class MovieService
    {
        private readonly string _connectionString; 

        private readonly string getAllMoviesSP = "getMovies";
        private readonly string findMovieQuery = "Select * From movies.movie Where movie_id = @id";
        private readonly string hightestVotedMoviesQuery = "Select * FROM movies.movie Order by vote_count desc Limit 10";
        private readonly string getRandomMoviesQuery = "SELECT * FROM movies.movie ORDER BY Rand() LIMIT 10";

        public MovieService(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<MovieModel>> GetMovies()
        {
            using(var sqlConnection = new MySqlConnection(_connectionString))
                return await sqlConnection.QueryAsync<MovieModel>(getAllMoviesSP, CommandType.StoredProcedure);
        }
        public async Task<MovieDetailsModel> FindMovie(int id)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("id", id, DbType.Int64);
                return await sqlConnection.QueryFirstAsync<MovieDetailsModel>(findMovieQuery, parameters);
            }
        }
        public async Task<IEnumerable<MovieModel>> GetHighestVotedMoviesAsync()
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
                return await sqlConnection.QueryAsync<MovieModel>(hightestVotedMoviesQuery);
        }
        public async Task<IEnumerable<MovieModel>> GetRandomMoviesAsync()
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
                return await sqlConnection.QueryAsync<MovieModel>(getRandomMoviesQuery);
        }
    }
}
