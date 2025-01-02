using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Public.FA.Data
{
    public static class DatabaseHelper
    {


        /// <summary>
        /// Thực thi câu lệnh SQL và trả về giá trị scalar (một giá trị đơn lẻ) với hỗ trợ timeout và cancellation token từ bên ngoài.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của giá trị trả về</typeparam>
        /// <typeparam name="conn">Chuỗi kết nối chuyền vào</typeparam>
        /// <param name="sql">Câu lệnh SQL cần thực thi</param>
        /// <param name="parameters">Các tham số truyền vào truy vấn (nếu có)</param>
        /// <param name="timeout">Thời gian tối đa cho phép trước khi hủy (tính bằng mili giây)</param>
        /// <param name="cancellationToken">CancellationToken từ bên ngoài để hủy yêu cầu</param>
        /// <returns>Giá trị scalar kiểu T</returns>
        public static T ExecuteScalar<T>(string conn, string sql, object parameters = null, int timeout = 30000)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sql));
            }


            try
            {
                using (var connection = new NpgsqlConnection(conn))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        // Thêm các tham số nếu có
                        if (parameters != null)
                        {
                            foreach (var property in parameters.GetType().GetProperties())
                            {
                                var parameterName = "@" + property.Name;
                                var parameterValue = property.GetValue(parameters) ?? DBNull.Value;
                                command.Parameters.AddWithValue(parameterName, parameterValue);
                            }
                        }

                        // Thực thi câu lệnh SQL với hỗ trợ hủy
                        var result = command.ExecuteScalarAsync();

                        // Chuyển đổi kiểu dữ liệu trả về
                        if (result == null)
                        {
                            return default; // Trả về giá trị mặc định nếu là null
                        }

                        return (T)Convert.ChangeType(result, typeof(T));
                    }
                }
            }

            catch (NpgsqlException ex)
            {
                // Xử lý lỗi liên quan đến PostgreSQL
                throw new InvalidOperationException("PostgreSQL execution failed. See inner exception for details.", ex);
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                throw new InvalidOperationException("An error occurred while executing the scalar query.", ex);
            }
        }



        /// <summary>
        /// Thực thi câu lệnh SQL và trả về giá trị scalar (một giá trị đơn lẻ) với hỗ trợ timeout và cancellation token từ bên ngoài.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của giá trị trả về</typeparam>
        /// <param name="conn">Chuỗi kết nối chuyền vào</param>
        /// <param name="sql">Câu lệnh SQL cần thực thi</param>
        /// <param name="parameters">Các tham số truyền vào truy vấn (nếu có)</param>
        /// <param name="timeout">Thời gian tối đa cho phép trước khi hủy (tính bằng mili giây)</param>
        /// <param name="cancellationToken">CancellationToken từ bên ngoài để hủy yêu cầu</param>
        /// <returns>Giá trị scalar kiểu T</returns>
        public static async Task<T> ExecuteScalarAsync<T>(string conn, string sql, object parameters = null, int timeout = 30000, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sql));
            }

            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(timeout);  // Thiết lập timeout

            try
            {
                using (var connection = new NpgsqlConnection(conn))
                {
                    await connection.OpenAsync(cancellationTokenSource.Token);

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        // Thêm các tham số nếu có
                        if (parameters != null)
                        {
                            foreach (var property in parameters.GetType().GetProperties())
                            {
                                var parameterName = "@" + property.Name;
                                var parameterValue = property.GetValue(parameters) ?? DBNull.Value;
                                command.Parameters.AddWithValue(parameterName, parameterValue);
                            }
                        }

                        // Thực thi câu lệnh SQL với hỗ trợ hủy
                        var result = await command.ExecuteScalarAsync(cancellationTokenSource.Token);

                        // Chuyển đổi kiểu dữ liệu trả về
                        if (result == null || result == DBNull.Value)
                        {
                            return default; // Trả về giá trị mặc định nếu là null
                        }

                        return (T)Convert.ChangeType(result, typeof(T));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Lỗi timeout hoặc bị hủy bởi CancellationToken từ caller
                throw new TimeoutException("The operation was canceled due to timeout or external cancellation.");
            }
            catch (NpgsqlException ex)
            {
                // Xử lý lỗi liên quan đến PostgreSQL
                throw new InvalidOperationException("PostgreSQL execution failed. See inner exception for details.", ex);
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                throw new InvalidOperationException("An error occurred while executing the scalar query.", ex);
            }
        }


        /// <summary>
        /// Đọc dữ liệu từ bảng và trả về một danh sách các đối tượng, hỗ trợ hủy bỏ và timeout.
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng sẽ lưu trữ dữ liệu từ bảng.</typeparam>
        /// <param name="conn">Chuỗi kết nối chuyền váo</param>
        /// <param name="sql">Câu lệnh SQL để truy vấn dữ liệu.</param>
        /// <param name="parameters">Các tham số của câu lệnh SQL (nếu có).</param>
        /// <param name="cancellationToken">CancellationToken để hủy bỏ tác vụ.</param>
        /// <param name="timeoutInSeconds">Thời gian timeout (giây) để hủy câu lệnh nếu quá lâu.</param>
        /// <returns>Danh sách các đối tượng kiểu T.</returns>
        public static List<T> ReadData<T>(string conn,  string sql, object parameters = null, CancellationToken cancellationToken = default, int timeoutInSeconds = 30) where T : new()
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sql));
            }
            var result = new List<T>();

            // Bắt đầu giao dịch
            using (var connection = new NpgsqlConnection(conn))
            {
                NpgsqlTransaction transaction = null;
                try
                {
                    connection.OpenAsync();
                    transaction = connection.BeginTransaction();

                    using (var command = new NpgsqlCommand(sql, connection, transaction))
                    {
                        // Thiết lập timeout cho câu lệnh SQL
                        command.CommandTimeout = timeoutInSeconds;

                        // Thêm tham số vào câu lệnh SQL nếu có
                        if (parameters != null)
                        {
                            foreach (var property in parameters.GetType().GetProperties())
                            {
                                var parameterName = "@" + property.Name;
                                var parameterValue = property.GetValue(parameters) ?? DBNull.Value;
                                command.Parameters.AddWithValue(parameterName, parameterValue);
                            }
                        }

                        using (var reader = command.ExecuteReaderAsync(cancellationToken).Result) // ExecuteAsync kết hợp với cancellationToken
                        {
                            // Đọc dữ liệu và chuyển vào danh sách đối tượng T
                            while (reader.Read())
                            {
                                var obj = new T();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var property = obj.GetType().GetProperty(reader.GetName(i));
                                    if (property != null && reader[i] != DBNull.Value)
                                    {
                                        property.SetValue(obj, reader[i]);
                                    }
                                }
                                result.Add(obj);
                            }
                        }
                    }

                    // Cam kết giao dịch nếu thành công
                    transaction.Commit();
                }
                catch (NpgsqlException npgsqlEx)
                {
                    // Xử lý lỗi đặc biệt từ PostgreSQL, bao gồm lỗi kết nối, giao dịch, truy vấn, ...
                    if (transaction != null)
                    {
                        // Hủy giao dịch nếu có lỗi
                        transaction.Rollback();
                    }
                    Console.WriteLine($"Lỗi PostgreSQL: {npgsqlEx.Message}");
                    throw new InvalidOperationException("Lỗi khi thực thi câu lệnh SQL trong PostgreSQL.", npgsqlEx);
                }
                catch (InvalidOperationException invOpEx)
                {
                    // Xử lý các lỗi về trạng thái không hợp lệ (Ví dụ: kết nối, giao dịch bị đóng,...)
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    Console.WriteLine($"Lỗi InvalidOperationException: {invOpEx.Message}");
                    throw new InvalidOperationException("Lỗi do trạng thái không hợp lệ trong quá trình thực thi truy vấn.", invOpEx);
                }
                catch (OperationCanceledException)
                {
                    // Xử lý lỗi khi tác vụ bị hủy
                    Console.WriteLine("Tác vụ đã bị hủy.");
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    throw new InvalidOperationException("Câu lệnh SQL đã bị hủy do timeout hoặc yêu cầu hủy bỏ.");
                }
                catch (Exception ex)
                {
                    // Xử lý các lỗi chung khác
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    Console.WriteLine($"Lỗi tổng quát: {ex.Message}");
                    throw new InvalidOperationException("Có lỗi không xác định khi thực thi truy vấn.", ex);
                }
                finally
                {
                    // Đảm bảo đóng kết nối nếu có bất kỳ lỗi nào hoặc sau khi hoàn tất
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            return result;
        }

        public static async Task<DataTable> ReadDataAsync( string connectionString,
        string query,
        IEnumerable<NpgsqlParameter>? parameters = null,
        bool useTransaction = false,
        CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));

            var dataTable = new DataTable();

            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);

                NpgsqlTransaction? transaction = null;
                if (useTransaction)
                {
                    transaction = await connection.BeginTransactionAsync(cancellationToken);
                }

                try
                {
                    await using var command = new NpgsqlCommand(query, connection)
                    {
                        Transaction = transaction
                    };

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    command.CommandTimeout = 30; // Thời gian chờ tối đa 30 giây
                    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

                    dataTable.Load(reader);

                    // Commit giao dịch nếu thành công
                    if (transaction != null)
                    {
                        await transaction.CommitAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    // Rollback giao dịch nếu có lỗi
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                    }

                    Console.WriteLine($"Error while executing query: {ex.Message}");
                    throw;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was canceled.");
                throw; // Bảo toàn exception để xử lý phía trên
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}\nSQL State: {ex.SqlState}");
                throw;
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Query execution timed out: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid operation: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }

            return dataTable;
        }


        // Đọc dữ liệu mapping và trả về đối tượng
        /// <summary>
        /// Đọc dữ liệu từ cơ sở dữ liệu PostgreSQL và ánh xạ sang danh sách đối tượng kiểu T.
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng cần ánh xạ.</typeparam>
        /// <param name="connectionString">Chuỗi kết nối đến cơ sở dữ liệu.</param>
        /// <param name="query">Câu lệnh SQL cần thực thi.</param>
        /// <param name="parameters">Danh sách tham số (tùy chọn).</param>
        /// <param name="useTransaction">Xác định xem có sử dụng giao dịch hay không.</param>
        /// <param name="cancellationToken">Token để hủy lệnh nếu cần.</param>
        /// <returns>Danh sách các đối tượng kiểu T.</returns>
        public static async Task<List<T>> ReadDataAsync<T>(
            string connectionString,
            string query,
            IEnumerable<NpgsqlParameter>? parameters = null,
            bool useTransaction = false,
            CancellationToken cancellationToken = default) where T : new()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));

            var resultList = new List<T>();

            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);

                NpgsqlTransaction? transaction = null;
                if (useTransaction)
                {
                    transaction = await connection.BeginTransactionAsync(cancellationToken);
                }

                try
                {
                    await using var command = new NpgsqlCommand(query, connection)
                    {
                        Transaction = transaction
                    };

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    command.CommandTimeout = 30; // Thời gian chờ tối đa 30 giây
                    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

                    // Ánh xạ dữ liệu từ IDataReader sang danh sách đối tượng kiểu T
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var obj = MapReaderToEntity<T>(reader);
                        resultList.Add(obj);
                    }

                    // Commit giao dịch nếu thành công
                    if (transaction != null)
                    {
                        await transaction.CommitAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    // Rollback giao dịch nếu có lỗi
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                    }

                    Console.WriteLine($"Error while executing query: {ex.Message}");
                    throw;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was canceled.");
                throw;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}\nSQL State: {ex.SqlState}");
                throw;
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Query execution timed out: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid operation: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }

            return resultList;
        }

        /// <summary>
        /// Ánh xạ dữ liệu từ IDataReader sang đối tượng kiểu T.
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng cần ánh xạ.</typeparam>
        /// <param name="reader">IDataReader.</param>
        /// <returns>Đối tượng kiểu T.</returns>
        private static T MapReaderToEntity<T>(IDataReader reader) where T : new()
        {
            var obj = new T();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (!reader.HasColumn(property.Name) || reader[property.Name] is DBNull) continue;

                var value = reader[property.Name];
                property.SetValue(obj, value);
            }

            return obj;
        }

        /// <summary>
        /// Kiểm tra xem cột có tồn tại trong IDataReader hay không.
        /// </summary>
        /// <param name="reader">IDataReader.</param>
        /// <param name="columnName">Tên cột.</param>
        /// <returns>True nếu tồn tại, ngược lại False.</returns>
        private static bool HasColumn(this IDataReader reader, string columnName)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }


        // Hàm ExcuteNonquery
        /// <summary>
        /// Thực thi lệnh SQL không trả về dữ liệu (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối đến cơ sở dữ liệu.</param>
        /// <param name="query">Câu lệnh SQL cần thực thi.</param>
        /// <param name="parameters">Danh sách tham số (tùy chọn).</param>
        /// <param name="useTransaction">Xác định xem có sử dụng giao dịch hay không.</param>
        /// <param name="cancellationToken">Token để hủy lệnh nếu cần.</param>
        /// <returns>Số dòng bị ảnh hưởng.</returns>
        public static async Task<int> ExecuteNonQueryAsync( string connectionString, string query, IEnumerable<NpgsqlParameter>? parameters = null, bool useTransaction = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));

            int rowsAffected = 0;

            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);

                NpgsqlTransaction? transaction = null;
                if (useTransaction)
                {
                    transaction = await connection.BeginTransactionAsync(cancellationToken);
                }

                try
                {
                    await using var command = new NpgsqlCommand(query, connection)
                    {
                        Transaction = transaction
                    };

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    command.CommandTimeout = 30; // Thời gian chờ tối đa 30 giây
                    rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);

                    // Commit giao dịch nếu thành công
                    if (transaction != null)
                    {
                        await transaction.CommitAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    // Rollback giao dịch nếu có lỗi
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                    }

                    Console.WriteLine($"Error while executing query: {ex.Message}");
                    throw;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was canceled.");
                throw;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}\nSQL State: {ex.SqlState}");
                throw;
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Query execution timed out: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid operation: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }

            return rowsAffected;
        }


        /// <summary>
        /// Thực thi trả về 1 dòng của dữ liệu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <param name="useTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<T> QuerySingleAsync<T>( string connectionString, string sqlQuery, object parameters = null, bool useTransaction = true, CancellationToken cancellationToken = default, int commandTimeout = 30) // Default timeout 30 seconds
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(sqlQuery))
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sqlQuery));

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            NpgsqlTransaction? transaction = null;
            if (useTransaction)
            {
                transaction = await connection.BeginTransactionAsync(cancellationToken);
            }

            try
            {
                // Thực thi câu lệnh SQL Query với timeout
                var command = new NpgsqlCommand(sqlQuery, connection, transaction);
                command.CommandTimeout = commandTimeout;
                // Thêm các tham số nếu có
                if (parameters != null)
                {
                    foreach (var property in parameters.GetType().GetProperties())
                    {
                        var parameterName = "@" + property.Name;
                        var parameterValue = property.GetValue(parameters) ?? DBNull.Value;
                        command.Parameters.AddWithValue(parameterName, parameterValue);
                    }
                }

                var result = await command.ExecuteScalarAsync(cancellationToken);

                if (transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }

                return (T)Convert.ChangeType(result, typeof(T));
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
            catch (TimeoutException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                Console.WriteLine($"The query timed out: {ex.Message}");
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
        }

        /// <summary>
        /// Trả về nhiều dòng
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <param name="useTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<List<T>> QueryMultipleAsync<T>( string connectionString, string sqlQuery, object parameters = null, bool useTransaction =true, CancellationToken cancellationToken = default, int commandTimeout = 30) // Default timeout 30 seconds
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(sqlQuery))
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sqlQuery));

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            NpgsqlTransaction? transaction = null;
            if (useTransaction)
            {
                transaction = await connection.BeginTransactionAsync(cancellationToken);
            }

            try
            {
                // Thực thi câu lệnh SQL với timeout
                var command = new NpgsqlCommand(sqlQuery, connection, transaction);
                command.CommandTimeout = commandTimeout;
                if (parameters != null)
                {
                    foreach (var property in parameters.GetType().GetProperties())
                    {
                        var parameterName = "@" + property.Name;
                        var parameterValue = property.GetValue(parameters) ?? DBNull.Value;
                        command.Parameters.AddWithValue(parameterName, parameterValue);
                    }
                }
                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                var result = new List<T>();

                while (await reader.ReadAsync())
                {
                    var item = Activator.CreateInstance<T>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var property = typeof(T).GetProperty(reader.GetName(i));
                        if (property != null && reader[i] != DBNull.Value)
                        {
                            property.SetValue(item, reader[i]);
                        }
                    }
                    result.Add(item);
                }

                if (transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }

                return result;
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
            catch (TimeoutException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                Console.WriteLine($"The query timed out: {ex.Message}");
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <param name="useTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<int> DeleteAsync(
    string connectionString,
    string sqlQuery,
    object parameters = null ,
    bool useTransaction = true,
    CancellationToken cancellationToken = default,
    int commandTimeout = 30) // Default timeout 30 seconds
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(sqlQuery))
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sqlQuery));

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            NpgsqlTransaction? transaction = null;
            if (useTransaction)
            {
                transaction = await connection.BeginTransactionAsync(cancellationToken);
            }

            try
            {
                // Thực thi câu lệnh SQL DELETE với timeout
                var command = new NpgsqlCommand(sqlQuery, connection, transaction);
                command.CommandTimeout = commandTimeout;

                if (parameters != null)
                {
                    foreach (var property in parameters.GetType().GetProperties())
                    {
                        var parameterName = "@" + property.Name;
                        var parameterValue = property.GetValue(parameters) ?? DBNull.Value;
                        command.Parameters.AddWithValue(parameterName, parameterValue);
                    }
                }
                // ExecuteDelete trả về số lượng dòng bị ảnh hưởng
                var result = await command.ExecuteNonQueryAsync(cancellationToken);

                if (transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }

                return result; // Trả về số dòng bị ảnh hưởng (số bản ghi bị xóa)
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
            catch (TimeoutException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                Console.WriteLine($"The query timed out: {ex.Message}");
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
        }
        /// <summary>
        /// Hàm chuyền bảng và điều kiện cần xóa
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="whereCondition"></param>
        /// <param name="parameters"></param>
        /// <param name="useTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<int> DeleteAsync(  string connectionString, string tableName, string whereCondition, object parameters = null, bool useTransaction = true, CancellationToken cancellationToken = default, int commandTimeout = 30) // Default timeout 30 seconds
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

            if (string.IsNullOrWhiteSpace(whereCondition))
                throw new ArgumentException("WHERE condition cannot be null or empty.", nameof(whereCondition));

            // Xây dựng câu lệnh SQL DELETE với điều kiện WHERE động
            string sqlQuery = $"DELETE FROM {tableName} WHERE {whereCondition}";

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            NpgsqlTransaction? transaction = null;
            if (useTransaction)
            {
                transaction = await connection.BeginTransactionAsync(cancellationToken);
            }

            try
            {
                // Thực thi câu lệnh SQL DELETE với timeout
                var command = new NpgsqlCommand(sqlQuery, connection, transaction);
                command.CommandTimeout = commandTimeout;

                // Thêm các tham số vào câu lệnh SQL
                if (parameters != null)
                {
                    foreach (var param in parameters.GetType().GetProperties())
                    {
                        var value = param.GetValue(parameters);
                        command.Parameters.AddWithValue(param.Name, value ?? DBNull.Value);
                    }
                }

                // ExecuteDelete trả về số lượng dòng bị ảnh hưởng
                var result = await command.ExecuteNonQueryAsync(cancellationToken);

                if (transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }

                return result; // Trả về số dòng bị ảnh hưởng (số bản ghi bị xóa)
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
            catch (TimeoutException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                Console.WriteLine($"The query timed out: {ex.Message}");
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
        }


        /// <summary>
        /// Lấy dữ liệu từ nhiều bảng // Chua dung
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="whereClause"></param>
        /// <param name="parameters"></param>
        /// <param name="useTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<List<T>> ReadDataAsync<T>(
                string connectionString,
                string tableName, // Tên bảng cần truy vấn
                string columns = "*", // Cột cần truy vấn, mặc định là tất cả
                string whereClause = "", // Điều kiện WHERE
                object parameters =null, // Tham số truyền vào
                bool useTransaction = true,
                CancellationToken cancellationToken = default,
                int commandTimeout = 30, // Thời gian timeout cho câu lệnh SQL
                int pageIndex = 1, // Số trang (mặc định là 1)
                int pageSize = 10 // Số bản ghi mỗi trang (mặc định là 10)
)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

            // Xây dựng câu lệnh SQL động
            var sqlQuery = $"SELECT {columns} FROM {tableName}";

            if (!string.IsNullOrEmpty(whereClause))
            {
                sqlQuery += " WHERE " + whereClause;
            }

            // Thêm phân trang
            int offset = (pageIndex - 1) * pageSize;
            sqlQuery += $" LIMIT {pageSize} OFFSET {offset}";

            // Mở kết nối đến cơ sở dữ liệu
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            NpgsqlTransaction? transaction = null;
            if (useTransaction)
            {
                transaction = await connection.BeginTransactionAsync(cancellationToken);
            }

            try
            {
                // Tạo command và thiết lập timeout
                var command = new NpgsqlCommand(sqlQuery, connection, transaction);
                command.CommandTimeout = commandTimeout;

                // Thêm tham số cho câu lệnh SQL
                if (parameters != null)
                {
                    foreach (var param in parameters.GetType().GetProperties())
                    {
                        var value = param.GetValue(parameters);
                        command.Parameters.AddWithValue(param.Name, value ?? DBNull.Value);
                    }
                }

                // Đọc dữ liệu trả về
                var reader = await command.ExecuteReaderAsync(cancellationToken);
                var result = new List<T>();

                while (await reader.ReadAsync(cancellationToken))
                {
                    var entity = Activator.CreateInstance<T>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var propertyName = reader.GetName(i);
                        var property = typeof(T).GetProperty(propertyName);

                        if (property != null && property.CanWrite)
                        {
                            var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            property.SetValue(entity, value);
                        }
                    }

                    result.Add(entity);
                }

                // Commit transaction nếu có
                if (transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }

                return result; // Trả về danh sách các đối tượng
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
            catch (TimeoutException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                Console.WriteLine($"The query timed out: {ex.Message}");
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
        }

    }


}

