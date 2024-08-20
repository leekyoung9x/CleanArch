using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class AccountRepository : BaseRepository<account>, IAccountRepository
    {
        public AccountRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<account?> Login(string username, string password)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                account? result = new account();
                try
                {
                    string tableName = GetTableName();
                    string keyColumn = GetKeyColumnName();
                    string query = $"SELECT * FROM {tableName} WHERE username = @username AND password = @password";

                    result = await connection.QuerySingleOrDefaultAsync<account>(query, new
                    {
                        username = username,
                        password = password,
                    });
                }
                catch (Exception ex) { }

                return result;
            }
        }
    }
}