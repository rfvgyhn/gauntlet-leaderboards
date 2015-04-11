using System.Xml.Serialization;

namespace GauntletLeaderboard.Core.Model
{
    [XmlRoot("entry")]
    public class Entry
    {
        [XmlElement("steamid")]  
        public long SteamId { get; set; }
        [XmlIgnore]
        public SteamProfile Player { get; set; }
        [XmlElement("score")]
        public int Score { get; set; }
        [XmlElement("rank")]
        public int Rank { get; set; }
        [XmlIgnore]
        public Leaderboard Leaderboard { get; set; }
    }
}