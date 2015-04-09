using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Api.Model
{
    public class SubGroup
    {
        public string Id { get { return Name.ToLower(); } }
        public string Group { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<Leaderboard> Leaderboards { get; set; }
    }
}