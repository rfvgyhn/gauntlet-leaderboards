using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Core.Model
{
    public class Player
    {
        public SteamProfile Profile { get; set; }
        public IEnumerable<Entry> Entries { get; set; }
        public int GlobalRank { get; set; }
        public int GlobalScore { get; set; }
    }
}
