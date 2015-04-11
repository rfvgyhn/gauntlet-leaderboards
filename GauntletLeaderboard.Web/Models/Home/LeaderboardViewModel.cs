using GauntletLeaderboard.Core;
using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Web.Models.Home
{
    public class LeaderboardViewModel : LeaderboardsViewModel, IPagedViewModel
    {
        public Leaderboard Leaderboard { get; set; }
        public IPagedResult<Entry> Entries { get; set; }

        public int PageIndex
        {
            get { return Entries.PageIndex; }
        }

        public int PageSize
        {
            get { return Entries.PageSize; }
        }

        public int? Next
        {
            get { return Entries.Next; }
        }

        public int? Previous
        {
            get { return Entries.Previous; }
        }

        public int First
        {
            get { return Entries.First; }
        }

        public int TotalPages
        {
            get { return Entries.PageCount; }
        }

        public int TotalItems
        {
            get { return Entries.TotalItemCount; }
        }
    }
}