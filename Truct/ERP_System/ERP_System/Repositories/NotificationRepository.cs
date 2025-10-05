using System.Data;
using System.Collections.Generic;
using ERP_System.Models;

namespace ERP_System.Repositories
{
    public class NotificationRepository : BaseRepository
    {
        public List<Notification> GetUserNotifications(int userID)
        {
            var notifications = new List<Notification>();
            var query = @"
                SELECT * FROM get_user_notifications(@user_id)";

            var parameters = new Dictionary<string, object>
            {
                { "@user_id", userID }
            };

            var dataTable = ExecuteQuery(query, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                notifications.Add(new Notification
                {
                    NotificationID = row.Field<int>("notification_id"),
                    Title = row.Field<string>("title"),
                    Message = row.IsDBNull("message") ? "" : row.Field<string>("message"),
                    NotificationType = row.Field<string>("notification_type"),
                    ReferenceType = row.Field<string>("reference_type"),
                    ReferenceID = row.Field<int>("reference_id"),
                    IsRead = row.Field<bool>("is_read"),
                    CreatedDate = row.Field<DateTime>("created_date")
                });
            }

            return notifications;
        }

        public bool MarkAsRead(int notificationID)
        {
            var query = "UPDATE notifications SET is_read = true WHERE notification_id = @notification_id";
            var parameters = new Dictionary<string, object>
            {
                { "@notification_id", notificationID }
            };

            return ExecuteNonQuery(query, parameters) > 0;
        }

        public int CreateNotification(Notification notification)
        {
            var query = @"
                SELECT create_notification(@user_id, @title, @message, @notification_type, @reference_type, @reference_id)";

            var parameters = new Dictionary<string, object>
            {
                { "@user_id", notification.UserID },
                { "@title", notification.Title },
                { "@message", notification.Message ?? (object)DBNull.Value },
                { "@notification_type", notification.NotificationType },
                { "@reference_type", notification.ReferenceType },
                { "@reference_id", notification.ReferenceID }
            };

            var result = ExecuteScalar(query, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}