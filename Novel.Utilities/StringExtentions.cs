using System;
using System.Linq;
using System.Linq.Expressions;

namespace Novel.Utilities
{
    public static class StringExtentions
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
    }
}
