using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GauntletLeaderboard.Core.Model
{
    [XmlRoot("entry")]
    public class Entry
    {
        const int DifficultyOffset = 1;
        const int CharacterOffset = 4;
        const int TeamOffset = 8;
        string detailsString;
        List<Toon> toons;

        [XmlElement("steamid")]  
        public long SteamId { get; set; }

        [XmlElement("details")]
        public string DetailsString {
            get
            {
                return detailsString;
            }
            set
            {
                detailsString = value;

                ParseAndSetDetails(value);
            }
        }

        [XmlElement("score")]
        public int Score { get; set; }

        [XmlElement("rank")]
        public int Rank { get; set; }

        [XmlIgnore]
        public Leaderboard Leaderboard { get; set; }

        [XmlIgnore]
        public byte[] Details { get; private set; }

        [XmlIgnore]
        public SteamProfile Player { get; set; }

        [XmlIgnore]
        public IEnumerable<Toon> Toons { get { return toons; } }

        [XmlIgnore]
        public Difficulty Difficulty { get; private set; }

        private void ParseAndSetDetails(string hexString)
        {
            Details = StringToByteArray(hexString);

            var toon = (Toon)Details[CharacterOffset];
            toons = new List<Toon>() { toon };
            var team = (Toon)Details[TeamOffset] & ~toon;

            foreach (Toon value in Enum.GetValues(team.GetType()))
            {
                if (team.HasFlag(value))
                    toons.Add(value);
            }
            Difficulty = (Difficulty)Details[DifficultyOffset];
        }

        // http://stackoverflow.com/a/311179/182821
        private static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}