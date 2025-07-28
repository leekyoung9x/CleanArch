using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class ItemTemplateRepository : BaseRepository<ItemTemplate>, IItemTemplateRepository
    {
        public ItemTemplateRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<ItemTemplate> GetByIdWithOptionsAsync(int id)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT it.*, iot.id as OptionId, iot.NAME as OptionName, iot.TYPE as OptionType
                        FROM item_template it
                        LEFT JOIN item_option_template iot ON it.TYPE = iot.TYPE
                        WHERE it.id = @Id";

                    var itemDict = new Dictionary<int, ItemTemplate>();

                    var result = await connection.QueryAsync<ItemTemplate, ItemOptionTemplate, ItemTemplate>(
                        query,
                        (item, option) =>
                        {
                            if (!itemDict.TryGetValue(item.Id, out var itemEntry))
                            {
                                itemEntry = item;
                                itemEntry.ItemOptionTemplates = new List<ItemOptionTemplate>();
                                itemDict.Add(item.Id, itemEntry);
                            }

                            if (option != null && option.Id > 0)
                            {
                                itemEntry.ItemOptionTemplates.Add(option);
                            }

                            return itemEntry;
                        },
                        new { Id = id },
                        splitOn: "OptionId"
                    );

                    return itemDict.Values.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    // Log exception
                    return null;
                }
            }
        }

        public async Task<IEnumerable<ItemTemplate>> GetByIdsWithOptionsAsync(IEnumerable<int> ids)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT it.*, iot.id as OptionId, iot.NAME as OptionName, iot.TYPE as OptionType
                        FROM item_template it
                        LEFT JOIN item_option_template iot ON it.TYPE = iot.TYPE
                        WHERE it.id IN @Ids
                        ORDER BY it.id";

                    var itemDict = new Dictionary<int, ItemTemplate>();

                    await connection.QueryAsync<ItemTemplate, ItemOptionTemplate, ItemTemplate>(
                        query,
                        (item, option) =>
                        {
                            if (!itemDict.TryGetValue(item.Id, out var itemEntry))
                            {
                                itemEntry = item;
                                itemEntry.ItemOptionTemplates = new List<ItemOptionTemplate>();
                                itemDict.Add(item.Id, itemEntry);
                            }

                            if (option != null && option.Id > 0)
                            {
                                itemEntry.ItemOptionTemplates.Add(option);
                            }

                            return itemEntry;
                        },
                        new { Ids = ids },
                        splitOn: "OptionId"
                    );

                    return itemDict.Values;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<ItemTemplate>();
                }
            }
        }
    }
}
