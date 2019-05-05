using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Novel.Utilities
{
    public static class EnumExtentions
    {
        public static string ToDescription(this Enum value)
        {
            if (value == null)
                return "";

            System.Reflection.FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);
            if (attribArray.Length == 0)
                return value.ToString();
            else
                return (attribArray[0] as DescriptionAttribute).Description;

        }
    }
}
