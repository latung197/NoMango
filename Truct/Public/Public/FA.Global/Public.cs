using Npgsql;
using Public.FA.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Public.FA.Global
{
    public class Public
    {

        // Test Hướng dẫn sử dụng Hàm
        public async void test()
        {
            var connectionString = "Host=localhost;Port=5432;Userna me=postgres;Password=yourpassword;Database=ERPMAX";
            // Hàm ExecuteScalarAsync
            // Tạo CancellationTokenSource bên ngoài
            var cancellationTokenSource = new CancellationTokenSource();
            // Ví dụ: Truy vấn có timeout 10 giây
            string sql = "SELECT pg_sleep(15); SELECT COUNT(*) FROM "; // pg_sleep để kiểm tra timeout

            try
            {
                // Truyền token vào hàm
                int userCount = await DatabaseHelper.ExecuteScalarAsync<int>("", sql, timeout: 10000, cancellationToken: cancellationTokenSource.Token); // Timeout 10 giây
                Console.WriteLine($"Tổng số người dùng: {userCount}");
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Truy vấn bị hủy do quá thời gian chờ.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Truy vấn đã bị hủy.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
            }

            // Bạn có thể gọi cancellationTokenSource.Cancel() từ bên ngoài để hủy yêu cầu sớm nếu cần
            // cancellationTokenSource.Cancel(); // Uncomment để hủy ngay lập tức

            // Hàm 
        }

        // Test ExecuteNonQueryAsync
        public async void testExcuteNonquery()
        {
            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=yourpassword;Database=yourdatabase";
            string query = "INSERT INTO your_table (column1, column2) VALUES (@value1, @value2)";
            var parameters = new[]
            {
            new NpgsqlParameter("@value1", "Sample Value 1"),
            new NpgsqlParameter("@value2", "Sample Value 2")
        };

            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Hủy sau 10 giây nếu lệnh quá lâu
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(100));

                int rowsAffected = await DatabaseHelper.ExecuteNonQueryAsync(
                    connectionString,
                    query,
                    parameters,
                    useTransaction: true,
                    cancellationToken: cancellationTokenSource.Token
                );

                Console.WriteLine($"Rows affected: {rowsAffected}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was canceled by the user.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async void TestInsert()
        {
            var jsonData = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "id", 1 },
                    { "name", "Alice" },
                    { "age", 25 }
                },
                new Dictionary<string, object>
                {
                    { "id", 2 },
                    { "name", "Bob" },
                    { "age", 30 }
                }
            };
            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=yourpassword;Database=yourdatabase";
            string tableName = "users";

            try
            {
                int rowsInserted = await DbStruct.InsertDataAsync(
                    connectionString,
                    tableName,
                    jsonData,
                    useTransaction: true
                );

                Console.WriteLine($"Rows inserted: {rowsInserted}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void TestUpdateSomeColumn()
        {
            var jsonData = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "id", 1 },
                    { "name", "Alice Updated" },
                    { "age", 26 }
                }
            };

            var fieldsToUpdate = new List<string> { "name", "age" };
        }
    }
}
