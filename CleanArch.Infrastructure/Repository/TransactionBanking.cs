using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class TransactionBanking : BaseRepository<transaction_banking>, ITransactionBanking
    {
        public TransactionBanking(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<transaction_banking>> GetTransactionBankingByPlayerId(int id)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                List<transaction_banking> result = new List<transaction_banking>();
                try
                {
                    string tableName = GetTableName();
                    string keyColumn = GetKeyColumnName();
                    string query = @"SELECT * FROM transaction_banking WHERE player_id = @id ORDER BY created_date DESC;";

                    result = (await connection.QueryAsync<transaction_banking>(query, new
                    {
                        id = id
                    })).ToList();
                }
                catch (Exception ex) { }

                return result;
            }
        }

        public async Task<transaction_banking?> GetTransactionByCustom(int id, int amount, string? otp)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                transaction_banking? result = new transaction_banking();
                try
                {
                    string tableName = GetTableName();
                    string keyColumn = GetKeyColumnName();
                    string query = @"SELECT * FROM transaction_banking WHERE player_id = @id AND amount = @amount AND description = @description;";

                    result = (await connection.QueryFirstOrDefaultAsync<transaction_banking>(query, new
                    {
                        id = id,
                        amount = amount,
                        description = otp,
                    }));
                }
                catch (Exception ex) { }

                return result;
            }
        }
    }
}
