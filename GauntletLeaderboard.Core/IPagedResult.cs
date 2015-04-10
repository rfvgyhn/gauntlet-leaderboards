using System.Collections.Generic;

namespace GauntletLeaderboard.Core
{
    public interface IPagedResult<out T> : IEnumerable<T>
    {
        int First { get; }
        int Last { get; }
        int? Next { get; }
        int? Previous { get; }
        int PageCount { get; }
        int TotalItemCount { get; }
        int PageIndex { get; }
        int PageSize { get; }
        IEnumerable<T> Page { get; }
    }
}