namespace Fischless.Design.Converters.Services;

internal interface ITimeZoneInfo
{
    public TimeZoneInfo Utc { get; }

    public TimeZoneInfo Local { get; }
}
