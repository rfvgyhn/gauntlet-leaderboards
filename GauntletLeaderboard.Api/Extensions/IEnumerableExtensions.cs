using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GauntletLeaderboard.Api;

namespace GauntletLeaderboard.Api.Extensions
{
    public static class IEnumerableExtensions
    {
        public static string JoinWith<T>(this IEnumerable<T> values, string separator)
        {
            return string.Join(separator, values);
        }

        public static IPagedResult<T> ToPagedResult<T>(this IEnumerable<T> items, int currentPage, int pageSize, int totalItems)
        {
            return new PagedResult<T>(items, currentPage, pageSize, totalItems);
        }
    }
}