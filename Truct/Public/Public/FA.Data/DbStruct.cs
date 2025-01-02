using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Public.FA.Data
{
    public class DbStruct
    {

        private NpgsqlConnection eval_a = new NpgsqlConnection();

        private string eval_b = "";

        private int eval_c = -1;

        private string eval_d;

        private string eval_e;

        private string eval_f;

        private DataTable eval_g = new DataTable("Table_Schema");

        private DataTable eval_h = new DataTable("PrimaryKeys_Schema");

        public string InsertString = "";

        public string SelectString = "";

        public string UpdateString = "";

        public string DeleteString = "";

        private bool eval_i;

        private string eval_k = string.Empty;

        public string strSQL = string.Empty;

        public string strSQLJoin = string.Empty;

        public List<string> listColumnNameAliasCheck = new List<string>();

        public void DBStruc(NpgsqlConnection cn, string TableName, int First, string Where, string Group, string Order, string SelectField)
        {
            this.eval_a = cn;
            this.eval_b = TableName.ToUpper();
            this.eval_c = First;
            this.eval_d = Where;
            if (this.eval_b == string.Empty || this.eval_b == null)
            {
                this.eval_b = "1 = 1";
            }
            this.eval_e = Group;
            this.eval_f = Order;
            this.eval_k = SelectField;
            NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(string.Format("SELECT TABLE_NAME, COLUMN_NAME, UDT_NAME, DOMAIN_NAME, NUMERIC_PRECISION, NUMERIC_SCALE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH FROM schema_columns WHERE table_name = '{0}' order by ordinal_position", this.eval_b.ToLower()), cn);
            npgsqlDataAdapter.Fill(this.eval_g);
            npgsqlDataAdapter = new NpgsqlDataAdapter(string.Format("SELECT TABLE_NAME, COLUMN_NAME FROM schema_key_column_usage WHERE table_name = '{0}'", this.eval_b.ToLower()), cn);
            npgsqlDataAdapter.Fill(this.eval_h);
        }

        public void DBStruc(NpgsqlConnection cn, string TableName, int First, string Where, string Group, string Order)
        {
            this.eval_a = cn;
            this.eval_b = TableName.ToUpper();
            this.eval_c = First;
            this.eval_d = Where;
            if (this.eval_b == string.Empty || this.eval_b == null)
            {
                this.eval_b = "1 = 1";
            }
            this.eval_e = Group;
            this.eval_f = Order;
            NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(string.Format("SELECT TABLE_NAME, COLUMN_NAME, UDT_NAME, DOMAIN_NAME, NUMERIC_PRECISION, NUMERIC_SCALE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH FROM schema_columns WHERE table_name = '{0}' order by ordinal_position", this.eval_b.ToLower()), cn);
            npgsqlDataAdapter.Fill(this.eval_g);
            npgsqlDataAdapter = new NpgsqlDataAdapter(string.Format("SELECT TABLE_NAME, COLUMN_NAME FROM schema_key_column_usage WHERE table_name = '{0}'", this.eval_b.ToLower()), cn);
            npgsqlDataAdapter.Fill(this.eval_h);
        }

        public void DBStruc(NpgsqlConnection cn, string TableName)
        {
            this.eval_a = cn;
            this.eval_b = TableName.ToUpper();
            NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(string.Format("SELECT TABLE_NAME, COLUMN_NAME, UDT_NAME, NUMERIC_PRECISION, NUMERIC_SCALE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH FROM schema_columns WHERE table_name = '{0}' order by ordinal_position", this.eval_b.ToLower()), cn);
            npgsqlDataAdapter.Fill(this.eval_g);
            npgsqlDataAdapter = new NpgsqlDataAdapter(string.Format("SELECT TABLE_NAME, COLUMN_NAME FROM schema_key_column_usage WHERE table_name = '{0}'", this.eval_b.ToLower()), cn);
            npgsqlDataAdapter.Fill(this.eval_h);
        }



        /// <summary>
        /// Sinh và thực thi câu lệnh INSERT từ JSON với hỗ trợ Transaction và xử lý lỗi đầy đủ.
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối đến database.</param>
        /// <param name="tableName">Tên bảng.</param>
        /// <param name="jsonData">Dữ liệu JSON (danh sách từ điển).</param>
        /// <param name="useTransaction">Xác định xem có sử dụng Transaction hay không.</param>
        /// <param name="cancellationToken">Token để hủy lệnh nếu cần.</param>
        /// <returns>Số dòng đã được chèn vào.</returns>
        public static async Task<int> InsertDataAsync(
            string connectionString,
            string tableName,
            List<Dictionary<string, object>> jsonData,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

            if (jsonData == null || !jsonData.Any())
                throw new ArgumentException("JSON data cannot be null or empty.", nameof(jsonData));

            int rowsInserted = 0;

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            NpgsqlTransaction? transaction = null;
            if (useTransaction)
            {
                transaction = await connection.BeginTransactionAsync(cancellationToken);
            }

            try
            {
                // Lấy danh sách cột từ schema của bảng
                var columns = await GetTableColumnsAsync(connection, tableName, cancellationToken);

                if (!columns.Any())
                    throw new InvalidOperationException($"Table '{tableName}' does not exist or has no columns.");

                // Tạo câu lệnh INSERT
                var insertCommandText = GenerateInsertStatement(tableName, columns, jsonData);

                await using var command = new NpgsqlCommand(insertCommandText, connection)
                {
                    Transaction = transaction
                };

                // Thực thi lệnh
                rowsInserted = await command.ExecuteNonQueryAsync(cancellationToken);

                if (transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                Console.WriteLine("Operation was canceled.");
                throw;
            }
            catch (NpgsqlException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                Console.WriteLine($"Database error: {ex.Message}\nSQL State: {ex.SqlState}");
                throw;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }

            return rowsInserted;
        }

        /// <summary>
        /// Lấy danh sách cột từ schema của bảng trong PostgreSQL.
        /// </summary>
        private static async Task<List<string>> GetTableColumnsAsync(
            NpgsqlConnection connection,
            string tableName,
            CancellationToken cancellationToken)
        {
            var columns = new List<string>();

            string schemaQuery = $@"
            SELECT column_name
            FROM information_schema.columns
            WHERE table_name = @tableName AND table_schema = 'public'
            ORDER BY ordinal_position;";

            await using var command = new NpgsqlCommand(schemaQuery, connection);
            command.Parameters.AddWithValue("@tableName", tableName);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                columns.Add(reader.GetString(0));
            }

            return columns;
        }

        /// <summary>
        /// Sinh câu lệnh INSERT từ dữ liệu JSON và danh sách cột.
        /// </summary>
        private static string GenerateInsertStatement(
            string tableName,
            List<string> columns,
            List<Dictionary<string, object>> jsonData)
        {
            var insertBuilder = new StringBuilder();
            insertBuilder.AppendLine($"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES");

            var valuesList = new List<string>();

            foreach (var row in jsonData)
            {
                var values = columns.Select(column =>
                {
                    if (row.TryGetValue(column, out var value) && value != null)
                    {
                        return value is string ? $"'{value.ToString().Replace("'", "''")}'" : value.ToString();
                    }

                    return "NULL";
                });

                valuesList.Add($"({string.Join(", ", values)})");
            }

            insertBuilder.AppendLine(string.Join(",\n", valuesList) + ";");
            return insertBuilder.ToString();
        }

        /// <summary>
        /// Kiểm tra dữ liệu JSON có key bị trùng lặp không.
        /// </summary>
        private static void ValidateJsonData(List<Dictionary<string, object>> jsonData)
        {
            foreach (var row in jsonData)
            {
                var duplicateKeys = row.GroupBy(k => k.Key)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => g.Key)
                                       .ToList();

                if (duplicateKeys.Any())
                {
                    throw new ArgumentException($"JSON data contains duplicate keys: {string.Join(", ", duplicateKeys)}");
                }
            }
        }

        private static async Task<bool> CheckIfRecordExistsAsync(
    NpgsqlConnection connection,
    string checkQuery,
    CancellationToken cancellationToken)
        {
            await using var command = new NpgsqlCommand(checkQuery, connection);
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Sinh câu lệnh SQL để kiểm tra xem bản ghi có tồn tại hay không.
        /// </summary>
        private static string GenerateCheckExistQuery(
            string tableName,
            List<string> columns,
            List<Dictionary<string, object>> jsonData)
        {
            var conditions = new List<string>();

            foreach (var row in jsonData)
            {
                foreach (var column in columns)
                {
                    if (row.TryGetValue(column, out var value) && value != null)
                    {
                        // Điều kiện kiểm tra có thể thay đổi tuỳ theo cách bạn muốn so sánh
                        conditions.Add($"{column} = '{value.ToString().Replace("'", "''")}'");
                    }
                }
            }

            // Tạo câu lệnh WHERE dựa trên dữ liệu JSON
            var whereClause = string.Join(" AND ", conditions);
            return $"SELECT COUNT(1) FROM {tableName} WHERE {whereClause}";
        }


        public static async Task<int> UpdateDataAsync( string connectionString, string tableName, List<Dictionary<string, object>> jsonData, bool useTransaction = true, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

            if (jsonData == null || !jsonData.Any())
                throw new ArgumentException("JSON data cannot be null or empty.", nameof(jsonData));

            // Kiểm tra key bị trùng lặp trong jsonData
            ValidateJsonData(jsonData);

            int rowsUpdated = 0;

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            NpgsqlTransaction? transaction = null;
            if (useTransaction)
            {
                transaction = await connection.BeginTransactionAsync(cancellationToken);
            }

            try
            {
                // Lấy danh sách cột từ schema của bảng
                var columns = await GetTableColumnsAsync(connection, tableName, cancellationToken);

                if (!columns.Any())
                    throw new InvalidOperationException($"Table '{tableName}' does not exist or has no columns.");

                // Kiểm tra xem bản ghi đã tồn tại trong cơ sở dữ liệu hay chưa
                var checkExistingQuery = GenerateCheckExistQuery(tableName, columns, jsonData);
                bool exists = await CheckIfRecordExistsAsync(connection, checkExistingQuery, cancellationToken);

                if (!exists)
                {
                    Console.WriteLine("Record does not exist, skipping UPDATE.");
                    return 0;  // Trả về 0 nếu bản ghi không tồn tại
                }

                // Tạo câu lệnh UPDATE
                var updateCommandText = GenerateUpdateStatement(tableName, columns, jsonData);

                await using var command = new NpgsqlCommand(updateCommandText, connection)
                {
                    Transaction = transaction
                };

                // Thực thi lệnh UPDATE
                rowsUpdated = await command.ExecuteNonQueryAsync(cancellationToken);

                if (transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                Console.WriteLine("Operation was canceled.");
                throw;
            }
            catch (NpgsqlException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                Console.WriteLine($"Database error: {ex.Message}\nSQL State: {ex.SqlState}");
                throw;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }

            return rowsUpdated;
        }

        /// <summary>
        /// Sinh câu lệnh SQL để cập nhật bản ghi.
        /// </summary>
        private static string GenerateUpdateStatement(
            string tableName,
            List<string> columns,
            List<Dictionary<string, object>> jsonData)
        {
            var updateClauses = new List<string>();
            var whereClauses = new List<string>();

            foreach (var row in jsonData)
            {
                foreach (var column in columns)
                {
                    if (row.TryGetValue(column, out var value) && value != null)
                    {
                        // Sinh câu lệnh SET (cập nhật)
                        updateClauses.Add($"{column} = '{value.ToString().Replace("'", "''")}'");
                    }
                }

                // Điều kiện WHERE, thường là khóa chính hoặc trường duy nhất
                var keyValuePairs = row.Where(k => k.Key == "id").ToList();
                foreach (var keyValue in keyValuePairs)
                {
                    whereClauses.Add($"id = '{keyValue.Value}'");
                }
            }

            // Tạo câu lệnh UPDATE với phần WHERE
            string setClause = string.Join(", ", updateClauses);
            string whereClause = string.Join(" AND ", whereClauses);

            return $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
        }

        /// <summary>
        /// Updat một số trường nhất định
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="jsonData"></param>
        /// <param name="fieldsToUpdate"></param>
        /// <param name="useTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<int> UpdateSomeFieldsAsync(
    string connectionString,
    string tableName,
    List<Dictionary<string, object>> jsonData,
    List<string> fieldsToUpdate,
    bool useTransaction = true,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

            if (jsonData == null || !jsonData.Any())
                throw new ArgumentException("JSON data cannot be null or empty.", nameof(jsonData));

            if (fieldsToUpdate == null || !fieldsToUpdate.Any())
                throw new ArgumentException("Fields to update cannot be null or empty.", nameof(fieldsToUpdate));

            // Kiểm tra key bị trùng lặp trong jsonData
            ValidateJsonData(jsonData);

            int rowsUpdated = 0;

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            NpgsqlTransaction? transaction = null;
            if (useTransaction)
            {
                transaction = await connection.BeginTransactionAsync(cancellationToken);
            }

            try
            {
                // Lấy danh sách cột từ schema của bảng
                var columns = await GetTableColumnsAsync(connection, tableName, cancellationToken);

                if (!columns.Any())
                    throw new InvalidOperationException($"Table '{tableName}' does not exist or has no columns.");

                // Kiểm tra xem bản ghi đã tồn tại trong cơ sở dữ liệu hay chưa
                var checkExistingQuery = GenerateCheckExistQuery(tableName, columns, jsonData);
                bool exists = await CheckIfRecordExistsAsync(connection, checkExistingQuery, cancellationToken);

                if (!exists)
                {
                    Console.WriteLine("Record does not exist, skipping UPDATE.");
                    return 0;  // Trả về 0 nếu bản ghi không tồn tại
                }

                // Tạo câu lệnh UPDATE cho các trường cần cập nhật
                var updateCommandText = GenerateUpdateSomeFieldsStatement(tableName, fieldsToUpdate, jsonData);

                await using var command = new NpgsqlCommand(updateCommandText, connection)
                {
                    Transaction = transaction
                };

                // Thực thi lệnh UPDATE
                rowsUpdated = await command.ExecuteNonQueryAsync(cancellationToken);

                if (transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                Console.WriteLine("Operation was canceled.");
                throw;
            }
            catch (NpgsqlException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                Console.WriteLine($"Database error: {ex.Message}\nSQL State: {ex.SqlState}");
                throw;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }

            return rowsUpdated;
        }

        /// <summary>
        /// Sinh câu lệnh SQL để cập nhật các trường cụ thể.
        /// </summary>
        private static string GenerateUpdateSomeFieldsStatement(
            string tableName,
            List<string> fieldsToUpdate,
            List<Dictionary<string, object>> jsonData)
        {
            var updateClauses = new List<string>();
            var whereClauses = new List<string>();

            foreach (var row in jsonData)
            {
                foreach (var column in fieldsToUpdate)
                {
                    if (row.TryGetValue(column, out var value) && value != null)
                    {
                        // Sinh câu lệnh SET (cập nhật)
                        updateClauses.Add($"{column} = '{value.ToString().Replace("'", "''")}'");
                    }
                }

                // Điều kiện WHERE, thường là khóa chính hoặc trường duy nhất
                var keyValuePairs = row.Where(k => k.Key == "id").ToList();
                foreach (var keyValue in keyValuePairs)
                {
                    whereClauses.Add($"id = '{keyValue.Value}'");
                }
            }

            // Tạo câu lệnh UPDATE với phần WHERE
            string setClause = string.Join(", ", updateClauses);
            string whereClause = string.Join(" AND ", whereClauses);

            return $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
        }
    }
}
