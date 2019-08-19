using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace EnumConstraint.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .FirstOrDefault()?
                            .GetCustomAttribute<TAttribute>() ;
        }

        public static string GetDisplayName(this Enum enumValue)
        {
            var displayAttribute = enumValue.GetAttribute<DisplayAttribute>();
            if (displayAttribute != null)
                return displayAttribute.Name;

            return enumValue.ToString();
        }

    }
}
