using Npgsql;
using System.Configuration;
using System.Data;
using System.Collections.Generic;

namespace ERP_System.Repositories
{
    public abstract class BaseRepository
    {
        protected string ConnectionString => ConfigurationManager.ConnectionStrings["ERP_PostgreSQL"].ConnectionString;

        protected NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        protected async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object> parameters = null)
        {
            var dataTable = new DataTable();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        protected async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters = null)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        protected async Task<object> ExecuteScalarAsync(string query, Dictionary<string, object> parameters = null)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    return await cmd.ExecuteScalarAsync();
                }
            }
        }

        protected DataTable ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            return ExecuteQueryAsync(query, parameters).GetAwaiter().GetResult();
        }

        protected int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            return ExecuteNonQueryAsync(query, parameters).GetAwaiter().GetResult();
        }

        protected object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
        {
            return ExecuteScalarAsync(query, parameters).GetAwaiter().GetResult();
        }
    }
}