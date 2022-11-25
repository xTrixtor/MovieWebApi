using Dapper;
using MyAwesomeWebApi.Models;
using MyAwesomeWebApi.Models.Reset;
using MyAwesomeWebApi.Models.User;
using MySql.Data.MySqlClient;
using System.Data;

namespace MyAwesomeWebApi.DataStore
{
    public class ResetService
    {
        private readonly string _connectionString;

        private readonly string userPasswordResetQuery = "Insert Into movies.UserPasswordReset (userID, resetCode, expiresOn) values (@userID, @resetCode, @expiresOn)";
        private readonly string updatePasswordQuery = "UPDATE movies.user SET password_hash = @passwordHash, password_salt = @passwordSalt Where id = @userID";
        private readonly string selectResetViewQuery = "Select * From UserPasswordResetView Where id = @userID and resetCode = @resetCode";


        public ResetService(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public async Task<string> InsertResetCodeAsync(int userID)
        {
            var resetCode = CreateResetCode();

            using(var connection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("userID", userID);
                parameters.Add("resetCode", resetCode);
                parameters.Add("expiresOn", DateTime.Now.AddMinutes(10));
                await connection.ExecuteAsync(userPasswordResetQuery, parameters);
            }
            return resetCode;
        }

        public async Task<string> CreateNewPasswordAsync(int userID, string resetCode, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var resetPasswordParameters = new DynamicParameters();
                resetPasswordParameters.Add("userID", userID);
                resetPasswordParameters.Add("resetCode", resetCode);
                var resetUserPassword = await connection.QueryFirstOrDefaultAsync<ResetUserPassword>(selectResetViewQuery, resetPasswordParameters);

                if (resetUserPassword is null) { return "Dein User passt nicht zu deinem ResetCode"; }
                var resetCodeIsExpired = resetUserPassword.ExpiresOn < DateTime.Now ;
                if (resetCodeIsExpired)
                        return "Dein ResetCode ist abgelaufen!";
                else
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("userID", userID);
                    parameters.Add("passwordSalt", passwordSalt);
                    parameters.Add("passwordHash", passwordHash);
                    await connection.ExecuteAsync(updatePasswordQuery, parameters);
                    return "Dein Password wurde geändert!";
                }
            }
        }

        private string CreateResetCode()
        {
            var random = new Random();
            var codeLength = 16;
            var resetCode = string.Empty;
            for (var i = 0; i < codeLength; i++)
                resetCode += ((char)(random.Next(1, 26) + 64)).ToString().ToLower();

            return resetCode;
        }

        
    }
}
