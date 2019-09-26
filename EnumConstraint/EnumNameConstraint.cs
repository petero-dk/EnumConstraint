using EnumConstraint.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnumConstraint
{


    public class EnumNameConstraint<T> : EnumNameConstraintBase where T : struct, Enum
    {
        public EnumNameConstraint() : base(typeof(T))
        {
        }
    }

    public class EnumNameConstraint : EnumNameConstraintBase
    {
        public EnumNameConstraint(string enumType) : base(Type.GetType(enumType.Split('|')[0])) //this needs a little more work
        {
        }
    }

    public abstract class EnumNameConstraintBase : IRouteConstraint
    {
        private readonly IEnumerable<string> validOptions;

        public EnumNameConstraintBase(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentOutOfRangeException("type must be an Enum");

            validOptions = EnumHelper.GetStringValues(enumType);
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out object value) && value != null)
            {
                var result = validOptions.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase);                
                return result;
            }
            return false;
        }
    }


}
