using Dapper;
using MyAwesomeWebApi.Models.User;
using MySql.Data.MySqlClient;

namespace MyAwesomeWebApi.DataStore
{
    public class UserService
    {
        private readonly string _connectionString;

        private readonly string selectUserQuery = "SELECT * FROM movies.user Where username = @username";
        private readonly string selectUserByID = "SELECT * FROM movies.user Where id = @userID";
        private readonly string createUserQuery = "Insert into movies.user (username, role_id, password_salt, password_hash) values (@username, 2, @passwordSalt, @passwordHash)";

        public UserService(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public async Task<UserDto> SelectUserById(int id)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("userID",id, System.Data.DbType.Int32);
                return await sqlConnection.QueryFirstOrDefaultAsync<UserDto>(selectUserByID, parameters);
            }
        }

        public async Task<UserDto> SelectUserAsync(User user)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("username", user.Username, System.Data.DbType.String);
                return await sqlConnection.QueryFirstOrDefaultAsync<UserDto>(selectUserQuery, parameters);
            }
        }

        public async Task CreateUser(string username, byte[] passwordSalt, byte[] passwordHash)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("username", username, System.Data.DbType.String);
                parameters.Add("passwordSalt", passwordSalt, System.Data.DbType.Binary);
                parameters.Add("passwordHash", passwordHash, System.Data.DbType.Binary);
                await sqlConnection.QueryAsync(createUserQuery, parameters);
            }
        }
    }
}
