namespace AdnTestingSystem.Core.Utils;

using System.ComponentModel.DataAnnotations;

public static class TimeHelper
{
    public static DateTimeOffset ConvertToUtcPlus7(DateTimeOffset dateTimeOffset)
    {
        // UTC+7 is 7 hours ahead of UTC
        TimeSpan utcPlus7Offset = new(7, 0, 0);
        return dateTimeOffset.ToOffset(utcPlus7Offset);
    }

    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    public static DateTimeOffset? ParseToDateTimeOffset(string? dateTimeString, string format = "yyyy-MM-dd HH:mm:ss")
    {
        if (string.IsNullOrWhiteSpace(dateTimeString)) return null;
        if (DateTimeOffset.TryParseExact(dateTimeString, format,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTimeOffset result))
        {
            return result;
        }
        else
            throw new ValidationException("Đinh dạng thời gian không đúng. vui lòng thử lại với định dạng " + format +
                                          '!');
    }
    public static string ConvertDateTimeOffsetToString(DateTimeOffset? dateTime, string format = "yyyy-MM-dd HH:mm:ss")
    {
        return dateTime?.ToString(format, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }
}