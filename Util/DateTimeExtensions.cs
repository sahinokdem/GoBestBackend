namespace GoBest.Util;

public static class DateTimeExtensions
{
    public static DateTime AsUtc(this DateTime dt) =>
        dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
            _ => dt
        };
}
