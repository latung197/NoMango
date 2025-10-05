using Npgsql;
using System.Configuration;
using System.Collections.Generic;
using ERP_System.Models;
using System.Data;

namespace ERP_System.Utilities
{
    public class DatabaseHelper
    {
        private static string _connectionString;

        public static void Initialize()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ERP_PostgreSQL"]?.ConnectionString;
        }

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public static List<DatabaseConnection> GetDatabaseConnections()
        {
            var connections = new List<DatabaseConnection>();

            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(@"
                        SELECT connection_id, connection_name, server_name, database_name, 
                               user_name, password, company_id, company_name 
                        FROM database_connections dc 
                        INNER JOIN companies c ON dc.company_id = c.company_id 
                        WHERE dc.is_active = true", conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            connections.Add(new DatabaseConnection
                            {
                                ConnectionID = reader.GetInt32(0),
                                ConnectionName = reader.GetString(1),
                                ServerName = reader.GetString(2),
                                DatabaseName = reader.GetString(3),
                                UserName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Password = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                CompanyID = reader.GetInt32(6),
                                CompanyName = reader.GetString(7)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể tải danh sách database: {ex.Message}");
            }

            return connections;
        }

        public static bool TestConnection(string connectionString)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static DataTable ExecuteStoredProcedure(string procedureName, Dictionary<string, object> parameters = null)
        {
            var dataTable = new DataTable();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

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
    }
}