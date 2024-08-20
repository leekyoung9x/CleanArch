using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using CleanArch.Sql.Queries;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class ContactRepository : BaseRepository<Contact>, IContactRepository
    {
        #region ===[ Private Members ]=============================================================

        private readonly IConfiguration configuration;

        #endregion ===[ Private Members ]=============================================================

        #region ===[ Constructor ]=================================================================

        public ContactRepository(IConfiguration configuration) : base(configuration)
        {
            this.configuration = configuration;
        }

        #endregion ===[ Constructor ]=================================================================

        #region ===[ IContactRepository Methods ]==================================================

        public async Task<IReadOnlyList<Contact>> GetAllAsync()
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                var result = await connection.QueryAsync<Contact>(ContactQueries.AllContact);

                return result.ToList();
            }
        }

        public async Task<Contact> GetByIdAsync(long id)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                var result = await connection.QuerySingleOrDefaultAsync<Contact>(ContactQueries.ContactById, new { ContactId = id });

                return result;
            }
        }

        public async Task<string> AddAsync(Contact entity)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                var result = await connection.ExecuteAsync(ContactQueries.AddContact, entity);

                return result.ToString();
            }
        }

        public async Task<string> UpdateAsync(Contact entity)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                var result = await connection.ExecuteAsync(ContactQueries.UpdateContact, entity);

                return result.ToString();
            }
        }

        public async Task<string> DeleteAsync(long id)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                var result = await connection.ExecuteAsync(ContactQueries.DeleteContact, new { ContactId = id });

                return result.ToString();
            }
        }

        #endregion ===[ IContactRepository Methods ]==================================================
    }
}