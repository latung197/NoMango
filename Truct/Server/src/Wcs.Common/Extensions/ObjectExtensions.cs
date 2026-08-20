using System.Text.Json;

namespace Wcs.Common.Extensions;

/// <summary>
/// Extension methods cho object để xử lý conversion an toàn, đặc biệt với JsonElement từ serialization
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Convert object (có thể là JsonElement từ CAP serialization) sang Int16
    /// </summary>
    public static short ToInt16(this object? value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Không thể convert null sang Int16");

        // Nếu là JsonElement (từ CAP/MessageQueue serialization)
        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.Number => jsonElement.GetInt16(),
                JsonValueKind.String => short.Parse(jsonElement.GetString()!),
                JsonValueKind.True => 1,
                JsonValueKind.False => 0,
                _ => throw new InvalidCastException($"Không thể convert JsonElement với ValueKind {jsonElement.ValueKind} sang Int16")
            };
        }

        // Nếu là kiểu thông thường
        return Convert.ToInt16(value);
    }

    /// <summary>
    /// Convert object sang Int16, trả về giá trị mặc định nếu lỗi
    /// </summary>
    public static short ToInt16OrDefault(this object? value, short defaultValue = 0)
    {
        try
        {
            return value.ToInt16();
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Convert object (có thể là JsonElement từ CAP serialization) sang Int32
    /// </summary>
    public static int ToInt32(this object? value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Không thể convert null sang Int32");

        // Nếu là JsonElement (từ CAP/MessageQueue serialization)
        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.Number => jsonElement.GetInt32(),
                JsonValueKind.String => int.Parse(jsonElement.GetString()!),
                JsonValueKind.True => 1,
                JsonValueKind.False => 0,
                _ => throw new InvalidCastException($"Không thể convert JsonElement với ValueKind {jsonElement.ValueKind} sang Int32")
            };
        }

        // Nếu là kiểu thông thường
        return Convert.ToInt32(value);
    }

    /// <summary>
    /// Convert object sang Int32, trả về giá trị mặc định nếu lỗi
    /// </summary>
    public static int ToInt32OrDefault(this object? value, int defaultValue = 0)
    {
        try
        {
            return value.ToInt32();
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Convert object sang Boolean
    /// </summary>
    public static bool ToBoolean(this object? value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Không thể convert null sang Boolean");

        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Number => jsonElement.GetInt32() != 0,
                JsonValueKind.String => bool.Parse(jsonElement.GetString()!),
                _ => throw new InvalidCastException($"Không thể convert JsonElement với ValueKind {jsonElement.ValueKind} sang Boolean")
            };
        }

        return Convert.ToBoolean(value);
    }

    /// <summary>
    /// Convert object sang String
    /// </summary>
    public static string ToStringValue(this object? value)
    {
        if (value == null)
            return string.Empty;

        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.String => jsonElement.GetString() ?? string.Empty,
                JsonValueKind.Number => jsonElement.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => string.Empty,
                _ => jsonElement.ToString()
            };
        }

        return value.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Convert object sang Double
    /// </summary>
    public static double ToDouble(this object? value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Không thể convert null sang Double");

        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.Number => jsonElement.GetDouble(),
                JsonValueKind.String => double.Parse(jsonElement.GetString()!),
                _ => throw new InvalidCastException($"Không thể convert JsonElement với ValueKind {jsonElement.ValueKind} sang Double")
            };
        }

        return Convert.ToDouble(value);
    }
}

