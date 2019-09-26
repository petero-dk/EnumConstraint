using EnumConstraint.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace EnumConstraint.Helpers
{

    public static class EnumHelper
    {

        static MethodInfo enumTryParse;

        static EnumHelper()
        {
            enumTryParse = typeof(Enum).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "TryParse" && m.GetParameters().Length == 3 && m.IsGenericMethod)
                .First();
            
        }

        public static bool TryParse(
            Type enumType,
            string value,
            bool ignoreCase,
            out object enumValue)
        {
            MethodInfo genericEnumTryParse = enumTryParse.MakeGenericMethod(enumType);

            object[] args = new object[] { value, ignoreCase, Enum.ToObject(enumType, 0) };
            bool success = (bool)genericEnumTryParse.Invoke(null, args);
            enumValue = args[2];

            return success;
        }



        internal static IEnumerable<FieldInfo> GetValuesInfo(Type enumType)
        {
            return enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
        }

        public static IEnumerable<string> GetStringValues(Type enumType)
        {
            return Enum.GetValues(enumType).OfType<Enum>().Select(i => i.ToString());
        }

        public static IEnumerable<string> GetDisplayValues(Type enumType)
        {
            try
            {
                return GetValuesInfo(enumType).Select(i => i.GetCustomAttribute<DisplayAttribute>().Name).ToArray();
            }
            catch
            {
                return Enum.GetNames(enumType);
            }
        }

        public static object GetValueFromDisplay(Type enumType, string display)
        {
            return GetValueFromDisplay(enumType, display, StringComparison.CurrentCulture);

        }

        public static object GetValueFromDisplay(Type enumType, string display, StringComparison comparisonType)
        {
            var members = GetValuesInfo(enumType).Where(i => display.Equals(i.GetCustomAttribute<DisplayAttribute>()?.Name, comparisonType));
            var member = members?.FirstOrDefault();
            
            if (TryParse(enumType, member?.Name, true, out var enumDisplayValue))
                return enumDisplayValue;

            if (TryParse(enumType, display, true, out var enumValue))
                return enumValue;

            return enumType.Default();

        }

    }

    public static class EnumHelper<T> where T : struct, Enum
    {
        /*
        public static IList<T> GetValues(Enum value)
        {
            
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
            
        }
        */

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames()
        {
            return Enum.GetNames(typeof(T));
            //return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<T> GetValues()
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        /*
        public static IList<string> GetDisplayValues()
        {
            return GetValues().Select(i => GetDisplayValue(i)).ToList();
            //return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }*/

        public static IEnumerable<string> GetDisplayValues()
        {
            return EnumHelper.GetDisplayValues(typeof(T));
        }


        public static T GetValueFromDisplay(string display)
        {
            var members = GetValuesInfo().Where(i => i.GetCustomAttribute<DisplayAttribute>()?.Name == display);
            var member = members?.FirstOrDefault();

            if (Enum.TryParse<T>(member?.Name, out var enumDisplayValue))
                return enumDisplayValue;

            if (Enum.TryParse<T>(display, out var enumValue))
                return enumValue;

            return default(T);

        }

        private static IEnumerable<FieldInfo> GetValuesInfo()
        {
            return EnumHelper.GetValuesInfo(typeof(T));
        }

        /*
        public static string GetDisplayValue(T flag)
        {
            try
            {
                return flag.GetType()
                        .GetMember(flag.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        .Name;
            }
            catch
            {
                return flag.ToString();
            }
        }

    */
    }
}
