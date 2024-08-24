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