using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Api
{
    public interface IPagedResult<T> : IEnumerable<T>
    {
        int PageCount { get; }
        int TotalItemCount { get; }
        int PageIndex { get; }
        IEnumerable<T> Page { get; }
    }
}