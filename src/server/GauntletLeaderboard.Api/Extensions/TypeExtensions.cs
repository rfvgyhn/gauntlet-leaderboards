using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Api.Extensions
{
    public static class TypeExtensions
    {
        public static string GetInnerTypeName(this Type t)
        {
            var type = t.GetElementType() ?? t;

            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments();

                if (args.Length > 1)
                    throw new ArgumentException("This method only supports generics with one type", "t");

                return args[0].GetInnerTypeName();
            }

            return type.Name;
        }
    }
}