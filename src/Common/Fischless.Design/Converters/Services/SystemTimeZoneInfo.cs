namespace Fischless.Design.Converters.Services;

internal class SystemTimeZoneInfo : ITimeZoneInfo
{
    private static readonly Lazy<ITimeZoneInfo> Implementation = new Lazy<ITimeZoneInfo>(CreateInstance, LazyThreadSafetyMode.PublicationOnly);

    public static ITimeZoneInfo Current => Implementation.Value;

    private static ITimeZoneInfo CreateInstance()
    {
        return new SystemTimeZoneInfo();
    }

    private SystemTimeZoneInfo()
    {
    }

    public TimeZoneInfo Utc => TimeZoneInfo.Utc;

    public TimeZoneInfo Local => TimeZoneInfo.Local;
}
