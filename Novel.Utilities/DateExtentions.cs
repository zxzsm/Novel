using System;
using System.Collections.Generic;
using System.Text;

namespace Novel.Utilities
{
    public static class DateExtentions
    {
        public static DateTime AsDateTime(this DateTime? value, DateTime defaultTime)
        {
            return value.HasValue ? value.Value : defaultTime;
        }
        public static DateTime AsDateTime(this DateTime? value)
        {
            return AsDateTime(value, default(DateTime));
        }
    }
}
