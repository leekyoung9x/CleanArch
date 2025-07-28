using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class ItemOptionTemplateRepository : BaseRepository<ItemOptionTemplate>, IItemOptionTemplateRepository
    {
        public ItemOptionTemplateRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<ItemOptionTemplate>> GetByTypeAsync(int type)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT * FROM item_option_template 
                        WHERE TYPE = @Type
                        ORDER BY id";

                    var result = await connection.QueryAsync<ItemOptionTemplate>(query, new { Type = type });
                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<ItemOptionTemplate>();
                }
            }
        }

        public async Task<IEnumerable<ItemOptionTemplate>> GetByIdsAsync(IEnumerable<int> ids)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT * FROM item_option_template 
                        WHERE id IN @Ids
                        ORDER BY id";

                    var result = await connection.QueryAsync<ItemOptionTemplate>(query, new { Ids = ids });
                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<ItemOptionTemplate>();
                }
            }
        }
    }
}
