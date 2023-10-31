using System.Globalization;

namespace LoggingAuto.Extensions;

public static class DateTimeExtensions
{
    public static void InitISO8601CultureInfo(this IServiceCollection services)
    {
        var cultureInfo = new CultureInfo("en-US"); // Set the desired culture
        cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
        cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }
}