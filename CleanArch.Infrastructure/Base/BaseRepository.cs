using CleanArch.Application.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Text;

namespace CleanArch.Infrastructure.Base
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly IConfiguration configuration;

        public BaseRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<bool> AddAsync(T entity)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                int rowsEffected = 0;
                try
                {
                    string tableName = GetTableName();
                    string columns = GetColumns(excludeKey: true);
                    string properties = GetPropertyNames(excludeKey: true);
                    string query = $"INSERT INTO {tableName} ({columns}) VALUES ({properties})";

                    rowsEffected = await connection.ExecuteAsync(query, entity);
                }
                catch (Exception ex) { }

                return rowsEffected > 0 ? true : false;
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                int rowsEffected = 0;
                try
                {
                    string tableName = GetTableName();
                    string keyColumn = GetKeyColumnName();
                    string keyProperty = GetKeyPropertyName();
                    string query = $"DELETE FROM {tableName} WHERE {keyColumn} = @{keyProperty}";

                    rowsEffected = await connection.ExecuteAsync(query, entity);
                }
                catch (Exception ex) { }

                return rowsEffected > 0 ? true : false;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                int rowsEffected = 0;
                try
                {
                    string tableName = GetTableName();
                    string keyColumn = GetKeyColumnName();
                    string keyProperty = GetKeyPropertyName();
                    string query = $"DELETE FROM {tableName} WHERE {keyColumn} = @Id";

                    rowsEffected = await connection.ExecuteAsync(query, new { Id = id });
                }
                catch (Exception ex) { }

                return rowsEffected > 0 ? true : false;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                IEnumerable<T> result = null;
                try
                {
                    string tableName = GetTableName();
                    string query = $"SELECT * FROM {tableName}";

                    result = await connection.QueryAsync<T>(query);
                }
                catch (Exception ex) { }

                return result;
            }
        }

        public async Task<T?> GetByIdAsync(long Id)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                T? result = null;
                try
                {
                    string tableName = GetTableName();
                    string keyColumn = GetKeyColumnName();
                    string query = $"SELECT * FROM {tableName} WHERE {keyColumn} = '{Id}'";

                    result = await connection.QuerySingleOrDefaultAsync<T>(query);
                }
                catch (Exception ex) { }

                return result;
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                int rowsEffected = 0;
                try
                {
                    string tableName = GetTableName();
                    string keyColumn = GetKeyColumnName();
                    string keyProperty = GetKeyPropertyName();

                    StringBuilder query = new StringBuilder();
                    query.Append($"UPDATE {tableName} SET ");

                    foreach (var property in GetProperties(true))
                    {
                        var columnAttr = property.GetCustomAttribute<ColumnAttribute>();

                        string propertyName = property.Name;
                        string columnName = columnAttr.Name;

                        query.Append($"{columnName} = @{propertyName},");
                    }

                    query.Remove(query.Length - 1, 1);

                    query.Append($" WHERE {keyColumn} = @{keyProperty}");

                    rowsEffected = await connection.ExecuteAsync(query.ToString(), entity);
                }
                catch (Exception ex) { }

                return rowsEffected > 0 ? true : false;
            }
        }

        protected string GetTableName()
        {
            string tableName = "";
            var type = typeof(T);
            var tableAttr = type.GetCustomAttribute<TableAttribute>();
            if (tableAttr != null)
            {
                tableName = tableAttr.Name;
                return tableName;
            }

            return type.Name + "s";
        }

        public static string GetKeyColumnName()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object[] keyAttributes = property.GetCustomAttributes(typeof(KeyAttribute), true);

                if (keyAttributes != null && keyAttributes.Length > 0)
                {
                    object[] columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), true);

                    if (columnAttributes != null && columnAttributes.Length > 0)
                    {
                        ColumnAttribute columnAttribute = (ColumnAttribute)columnAttributes[0];
                        return columnAttribute.Name;
                    }
                    else
                    {
                        return property.Name;
                    }
                }
            }

            return null;
        }

        private string GetColumns(bool excludeKey = false)
        {
            var type = typeof(T);
            var columns = string.Join(", ", type.GetProperties()
                .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
                .Select(p =>
                {
                    var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                    return columnAttr != null ? columnAttr.Name : p.Name;
                }));

            return columns;
        }

        protected string GetPropertyNames(bool excludeKey = false)
        {
            var properties = typeof(T).GetProperties()
                .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

            var values = string.Join(", ", properties.Select(p =>
            {
                return $"@{p.Name}";
            }));

            return values;
        }

        protected IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false)
        {
            var properties = typeof(T).GetProperties()
                .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

            return properties;
        }

        protected string GetKeyPropertyName()
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<KeyAttribute>() != null);

            if (properties.Any())
            {
                return properties.FirstOrDefault().Name;
            }

            return null;
        }
    }
}