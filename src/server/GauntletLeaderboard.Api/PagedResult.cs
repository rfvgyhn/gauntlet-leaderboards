using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Api
{
    public class PagedResult<T> : IPagedResult<T>//, IEnumerable<T>
    {
        public int PageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageIndex { get; private set; }
        public IEnumerable<T> Page { get; private set; }

        public PagedResult(IEnumerable<T> items, int currentPage, int pageSize, int totalItems)
        {
            PageCount = totalItems % pageSize == 0 ? totalItems / pageSize : (totalItems / pageSize) + 1;
            TotalItemCount = totalItems;
            PageIndex = currentPage;
            Page = items;
        }

        //public IEnumerator<T> GetEnumerator()
        //{
        //    return Page.GetEnumerator();
        //}

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    return Page.GetEnumerator();
        //}
    }
}