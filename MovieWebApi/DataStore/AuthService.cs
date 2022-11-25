using Dapper;
using MyAwesomeWebApi.Models.User;
using MySql.Data.MySqlClient;
using System.Data;

namespace MyAwesomeWebApi.DataStore
{
    public class AuthService
    {
        private readonly string _connectionString;

        private readonly string getUserQUery = "SELECT * FROM movies.user Where username = @name";
        public AuthService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UserDto> GetUserAsync(User user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("name", user.Username, DbType.String);
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<UserDto>(getUserQUery,parameters);
            }
        }

        public string TestConnection()
        {
            try
            {
                using (var sqlConnection = new MySqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    return $"Connection Open";
                }
            }
            catch (Exception e)
            {
                return ($"{e.InnerException} {e.Message}");
                throw e.InnerException;
            }
        }
    }
}
