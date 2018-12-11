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
    }
}
