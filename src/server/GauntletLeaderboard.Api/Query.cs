using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Api
{
    public class Query
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}