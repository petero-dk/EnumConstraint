using EnumConstraint.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnumConstraint
{


    public class EnumDisplayNameConstraint<T> : EnumDisplayNameConstraintBase where T : struct, Enum
    {
        public EnumDisplayNameConstraint(bool changeValueToEnum) : base(typeof(T), changeValueToEnum)
        {
        }
    }

    public class EnumDisplayNameConstraint : EnumDisplayNameConstraintBase
    {
        public EnumDisplayNameConstraint(string enumType) : base(Type.GetType(enumType.Split('|')[0]), false) //this needs a little more work
        {
        }
    }

    public abstract class EnumDisplayNameConstraintBase : IRouteConstraint
    {
        private readonly IEnumerable<string> validOptions;
        private readonly bool changeValue;
        private readonly Type type;

        public EnumDisplayNameConstraintBase(Type enumType, bool changeValueToEnum)
        {
            if (!enumType.IsEnum)
                throw new ArgumentOutOfRangeException("type must be an Enum");

            validOptions = EnumHelper.GetDisplayValues(enumType);
            changeValue = changeValueToEnum;
            type = enumType;
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out object value) && value != null)
            {

                var result = validOptions.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase);
                if (result && changeValue)
                {
                    var enumValue = EnumHelper.GetValueFromDisplay(type, value.ToString(), StringComparison.OrdinalIgnoreCase);
                    values[routeKey] = enumValue.ToString();
                }
                return result;
            }
            return false;
        }
    }


}
