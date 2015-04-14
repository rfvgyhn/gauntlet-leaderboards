using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Web.Models.Home
{
    public class LeaderboardsViewModel : SubGroupsViewModel
    {
        public SubGroup SubGroup { get; set; }
        public IEnumerable<Leaderboard> Leaderboards { get; set; }
    }
}