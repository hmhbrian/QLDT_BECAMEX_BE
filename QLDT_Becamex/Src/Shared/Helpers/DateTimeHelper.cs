namespace QLDT_Becamex.Src.Shared.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        /// <summary>
        /// Chuyển đổi thời gian UTC sang múi giờ Việt Nam (UTC+07:00).
        /// </summary>
        /// <param name="utcDateTime">Thời gian UTC cần chuyển đổi.</param>
        /// <returns>Thời gian trong múi giờ Việt Nam.</returns>
        public static DateTime ToVietnamTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Input DateTime must be in UTC.", nameof(utcDateTime));
            }
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
        }

        /// <summary>
        /// Lấy thời gian hiện tại theo múi giờ Việt Nam.
        /// </summary>
        /// <returns>Thời gian hiện tại trong múi giờ Việt Nam.</returns>
        public static DateTime GetVietnamTimeNow()
        {
            return ToVietnamTime(DateTime.UtcNow);
        }
    }
}
