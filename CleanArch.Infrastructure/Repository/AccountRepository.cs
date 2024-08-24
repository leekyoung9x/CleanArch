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
                    string query = $"SELECT id, username, vnd, role, active, create_time FROM {tableName} WHERE username = @username AND password = @password";

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

        public async Task<bool> IsExist(string username)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                account? account = new account();
                string tableName = GetTableName();
                string keyColumn = GetKeyColumnName();
                string query = $"SELECT id FROM {tableName} WHERE username = @username";

                account = await connection.QuerySingleOrDefaultAsync<account>(query, new
                {
                    username = username,
                });

                if (account != null)
                {
                    return true;
                }

                return false;
            }
        }

        public async Task<bool> Register(string username, string password, string ip)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                string tableName = GetTableName();
                string keyColumn = GetKeyColumnName();
                string query = $"INSERT INTO {tableName}(`username`, `password`, `ip_address`) VALUES (@username, @password, @ip)";

                var result = await connection.ExecuteAsync(query, new
                {
                    username = username,
                    password = password,
                    ip = ip,
                });

                return result > 0;
            }
        }

        public async Task<bool> ChangePassword(int id, string passwordNew)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                string tableName = GetTableName();
                string keyColumn = GetKeyColumnName();
                string query = $"UPDATE {tableName} SET password = @password WHERE id = @id";

                var result = await connection.ExecuteAsync(query, new
                {
                    id = id,
                    password = passwordNew,
                });

                return result > 0;
            }
        }

        public async Task<int> GetPlayerIdByAccountId(int id)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                string tableName = GetTableName();
                string keyColumn = GetKeyColumnName();
                string query = $"SELECT b.id FROM {tableName} a INNER JOIN player b ON a.id = b.account_id WHERE a.id = @id";

                var result = await connection.QueryFirstOrDefaultAsync<int>(query, new
                {
                    id = id,
                });

                return result;
            }
        }
    }
}