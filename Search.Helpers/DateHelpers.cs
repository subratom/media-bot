using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.Helpers
{
    public static class DateHelpers
    {
        public static long ToUnixTimeSeconds(DateTime currentDate)
        {
            if (currentDate != null)
            {
                var dateTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, currentDate.Minute, currentDate.Second, DateTimeKind.Local);
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return (long)((dateTime.ToUniversalTime() - epoch).TotalSeconds);
            }
            else
            {
                return 0;
            }
        }

        public static DateTime ToDateTime(long seconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(seconds);
        }
    }
}
