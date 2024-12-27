namespace hoikun.Data
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Sunday)
        {
            int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
            return date.AddDays(-1 * diff).Date;
        }
    }

}
