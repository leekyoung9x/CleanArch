using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class RankRepository : BaseRepository<rank>, IRankRepository
    {
        public RankRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<rank>> GetPowerRank()
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                List<rank> result = new List<rank>();
                try
                {
                    string tableName = GetTableName();
                    string keyColumn = GetKeyColumnName();
                    string query = @"SELECT
                                      name,
                                      power AS `value`
                                    FROM player
                                    WHERE name <> 'admin'
                                    ORDER BY power DESC LIMIT 10";

                    result = (await connection.QueryAsync<rank>(query)).ToList();
                }
                catch (Exception ex) { }

                return result;
            }
        }
        
        public async Task<List<rank>> GetVndRank()
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                List<rank> result = new List<rank>();
                try
                {
                    string tableName = GetTableName();
                    string keyColumn = GetKeyColumnName();
                    string query = @"SELECT i.name, j.vnd as value FROM player i INNER JOIN (
                                    SELECT a.player_id, SUM(a.amount) AS vnd FROM (
                                    SELECT player_id, amount FROM transaction_banking
                                    WHERE is_recieve = 1
                                    UNION ALL
                                    SELECT player_id, amount_real FROM transaction_card
                                    WHERE status IN (1, 2)) a
                                    GROUP BY a.player_id
                                    ORDER BY vnd DESC
                                    LIMIT 0, 10) j ON i.id = j.player_id
                                    ORDER BY j.vnd DESC;";

                    result = (await connection.QueryAsync<rank>(query)).ToList();
                }
                catch (Exception ex) { }

                return result;
            }
        }

        public async Task<List<rank>> GetPetPowerRank()
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                List<rank> result = new List<rank>();
                try
                {
                    string tableName = GetTableName();
                    string keyColumn = GetKeyColumnName();
                    string query = @"SELECT
                                      name,
                                      pet_power AS `value`
                                    FROM player
                                    WHERE name <> 'admin'
                                    ORDER BY pet_power DESC LIMIT 10";

                    result = (await connection.QueryAsync<rank>(query)).ToList();
                }
                catch (Exception ex) { }

                return result;
            }
        }
    }
}