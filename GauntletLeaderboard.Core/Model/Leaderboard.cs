using System.Collections.Generic;

namespace GauntletLeaderboard.Core.Model
{
    public class Leaderboard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Group { get; set; }
        public string SubGroup { get; set; }
        public string Special { get; set; }
        public ScoreType ScoreType { get; set; }
        public IEnumerable<Entry> Entries { get; set; }
    }
}