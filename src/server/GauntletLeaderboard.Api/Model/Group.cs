using System.Collections.Generic;

namespace GauntletLeaderboard.Api.Model
{
    public class Group
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<Group> SubGroups { get; set; }
    }
}