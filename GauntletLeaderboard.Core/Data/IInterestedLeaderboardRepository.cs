using GauntletLeaderboard.Core.Model;
using System.Collections.Generic;

namespace GauntletLeaderboard.Core.Data
{
    public interface IInterestedLeaderboardRepository
    {
        IEnumerable<InterestedLeaderboard> GetLeaderboards();
    }
}
