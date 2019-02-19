using System;
using System.Linq;
using System.Linq.Expressions;

namespace Novel.Utilities
{
    public static class XExtentions
    {
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string AsTrim(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value.Trim();
        }

        public static int AsInt(this string value, int defaultValue = 0)
        {
            int m = 0; if (int.TryParse(value, out m)) return m; return defaultValue;
        }

        public static DateTime AsDateTime(this DateTime? value, DateTime defaultTime)
        {
            return value.HasValue ? value.Value : defaultTime;
        }
        public static DateTime AsDateTime(this DateTime? value)
        {
            return AsDateTime(value, default(DateTime));
        }

        public static int AsInt(this int? value, int defalultValue = 0)
        {
            return value.HasValue ? value.Value : 0;
        }
    }
}
