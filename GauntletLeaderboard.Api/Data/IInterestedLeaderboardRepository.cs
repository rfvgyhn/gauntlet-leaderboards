using GauntletLeaderboard.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GauntletLeaderboard.Api.Data
{
    public interface IInterestedLeaderboardRepository
    {
        IEnumerable<InterestedLeaderboard> GetLeaderboards();
    }
}
