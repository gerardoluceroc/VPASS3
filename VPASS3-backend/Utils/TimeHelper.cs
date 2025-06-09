namespace VPASS3_backend.Utils
{
    public static class TimeHelper
    {
        public static DateTime GetSantiagoTime()
        {
            try
            {
                return TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("America/Santiago")
                );
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")
                );
            }
        }
    }
}
