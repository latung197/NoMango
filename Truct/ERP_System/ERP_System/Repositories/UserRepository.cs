using System.Data;
using System.Collections.Generic;
using ERP_System.Models;
using ERP_System.Utilities;

namespace ERP_System.Repositories
{
    public class UserRepository : BaseRepository
    {
        public User Authenticate(string username, string password)
        {
            var query = @"
                SELECT * FROM authenticate_user(@username, @password)";

            var parameters = new Dictionary<string, object>
            {
                { "@username", username },
                { "@password", PasswordHelper.HashPassword(password) }
            };

            var dataTable = ExecuteQuery(query, parameters);

            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return new User
                {
                    UserID = row.Field<int>("user_id"),
                    UserName = row.Field<string>("user_name"),
                    FullName = row.Field<string>("full_name"),
                    Email = row.IsDBNull("email") ? "" : row.Field<string>("email"),
                    Phone = row.IsDBNull("phone") ? "" : row.Field<string>("phone"),
                    LanguageID = row.Field<int>("language_id"),
                    CompanyID = row.Field<int>("company_id"),
                    CompanyName = row.Field<string>("company_name")
                };
            }

            return null;
        }

        public List<User> GetUsers()
        {
            var users = new List<User>();
            var query = @"
                SELECT u.user_id, u.user_name, u.full_name, u.email, u.phone, 
                       u.language_id, u.company_id, c.company_name, u.is_active, u.created_date
                FROM users u
                INNER JOIN companies c ON u.company_id = c.company_id
                ORDER BY u.user_name";

            var dataTable = ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                users.Add(new User
                {
                    UserID = row.Field<int>("user_id"),
                    UserName = row.Field<string>("user_name"),
                    FullName = row.Field<string>("full_name"),
                    Email = row.IsDBNull("email") ? "" : row.Field<string>("email"),
                    Phone = row.IsDBNull("phone") ? "" : row.Field<string>("phone"),
                    LanguageID = row.Field<int>("language_id"),
                    CompanyID = row.Field<int>("company_id"),
                    CompanyName = row.Field<string>("company_name"),
                    IsActive = row.Field<bool>("is_active"),
                    CreatedDate = row.Field<DateTime>("created_date")
                });
            }

            return users;
        }

        public bool UpdateUser(User user)
        {
            var query = @"
                UPDATE users 
                SET full_name = @full_name, email = @email, phone = @phone, 
                    language_id = @language_id, is_active = @is_active
                WHERE user_id = @user_id";

            var parameters = new Dictionary<string, object>
            {
                { "@user_id", user.UserID },
                { "@full_name", user.FullName },
                { "@email", user.Email ?? (object)DBNull.Value },
                { "@phone", user.Phone ?? (object)DBNull.Value },
                { "@language_id", user.LanguageID },
                { "@is_active", user.IsActive }
            };

            return ExecuteNonQuery(query, parameters) > 0;
        }

        public bool ChangePassword(int userID, string newPassword)
        {
            var query = "UPDATE users SET password = @password WHERE user_id = @user_id";
            var parameters = new Dictionary<string, object>
            {
                { "@user_id", userID },
                { "@password", PasswordHelper.HashPassword(newPassword) }
            };

            return ExecuteNonQuery(query, parameters) > 0;
        }
    }
}