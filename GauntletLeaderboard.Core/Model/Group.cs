using System.Collections.Generic;
using GauntletLeaderboard.Core.Extensions;

namespace GauntletLeaderboard.Core.Model
{
    public class Group
    {
        public string Id { get { return Name.ToLower().ToSlug(); } }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<Group> SubGroups { get; set; }
    }
}