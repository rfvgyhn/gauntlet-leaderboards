using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Core.Model
{
    public class SteamBadge
    {
        public int AppId { get; set; }
        public int BadgeId { get; set; }
        public int Level { get; set; }
        [JsonProperty(PropertyName = "border_color")]
        public int BorderColor { get; set; }
    }
}
