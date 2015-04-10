using System.Xml.Serialization;

namespace GauntletLeaderboard.Core.Model
{
    [XmlRoot("entry")]
    public class Entry
    {
        [XmlElement("steamid")]  
        public string SteamId { get; set; }
        public SteamProfile Player { get; set; }
        [XmlElement("score")]
        public int Score { get; set; }
        [XmlElement("rank")]
        public int Rank { get; set; }
    }
}