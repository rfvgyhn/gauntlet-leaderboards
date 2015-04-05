using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Api
{
    public class Query
    {
        public Query()
        {
            Page = 0;
            PageSize = 20;
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}