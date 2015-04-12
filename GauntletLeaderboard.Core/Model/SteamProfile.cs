
using System.Collections.Generic;
namespace GauntletLeaderboard.Core.Model
{
    public enum PersonaState
    {
        Offline = 0,
        Online = 1,
        Busy = 2,
        Away = 3,
        Snooze = 4,
        LookingToTrade = 5,
        LookingToPlay = 6
    }
    public class SteamProfile
    {
        public long SteamId { get; set; }
        public string PersonaName { get; set; }
        public string ProfileUrl { get; set; }
        
        public string Avatar { get; set; }
        public string AvatarMedium { get; set; }
        public string AvatarFull { get; set; }
        public PersonaState PersonaState { get; set; }
        public float? AchievementPercentage { get; set; }
        public IEnumerable<SteamBadge> Badges { get; set; }

        public string VanityName
        {
            get
            {
                if (ProfileUrl.Contains("/id"))
                    return ProfileUrl.Substring(ProfileUrl.LastIndexOf("/id") + 4).TrimEnd('/');

                return null;
            }
        }
    }
}