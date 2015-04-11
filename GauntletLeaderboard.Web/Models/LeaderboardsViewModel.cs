using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Web.Models
{
    public class LeaderboardsViewModel : SubGroupsViewModel
    {
        public string Group { get; set; }
        public string SubGroup { get; set; }
        public IEnumerable<Leaderboard> Leaderboards { get; set; }
    }
}