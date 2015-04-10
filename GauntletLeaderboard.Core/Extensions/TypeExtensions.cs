using System;

namespace GauntletLeaderboard.Core.Extensions
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