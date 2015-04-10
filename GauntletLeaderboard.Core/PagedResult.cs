using System.Collections.Generic;

namespace GauntletLeaderboard.Core
{
    public class PagedResult<T> : IPagedResult<T>, IEnumerable<T>
    {
        public int PageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageIndex { get; private set; }
        public IEnumerable<T> Page { get; private set; }
        public int PageSize { get; private set; }
        public int First { get; private set; }
        public int Last { get; private set; }
        public int? Next { get; private set; }
        public int? Previous { get; private set; }

        public PagedResult(IEnumerable<T> items, int currentPage, int pageSize, int totalItems)
        {
            PageCount = totalItems % pageSize == 0 ? totalItems / pageSize : (totalItems / pageSize) + 1;
            TotalItemCount = totalItems;
            PageIndex = currentPage;
            Page = items;
            PageSize = pageSize;
            First = 1;
            Last = PageCount;
            Next = currentPage == Last ? (int?)null : currentPage + 1;
            Previous = currentPage == First ? (int?)null : currentPage - 1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Page.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Page.GetEnumerator();
        }
    }
}