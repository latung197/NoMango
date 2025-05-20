using System.Globalization;

namespace PlastMB.Utils
{
    public static class DateUtils
    {
        public static DateTime? GetDate(string dateString, string format = "yyyy/MM/dd")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dateString))
                    return null;

                if (!string.IsNullOrEmpty(format))
                {
                    string isoFormat = "yyyy-MM-ddTHH:mm:ssK";
                    var dto = DateTimeOffset.ParseExact(dateString, isoFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    return dto.DateTime; // hoặc dto.UtcDateTime nếu cần UTC
                    // Nếu truyền format cụ thể, dùng ParseExact
                }
                else
                {
                    // Format ISO 8601: yyyy-MM-ddTHH:mm:ssK
                    return DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);

                }

            }
            catch
            {
                return null;
            }
        }
    }
}
