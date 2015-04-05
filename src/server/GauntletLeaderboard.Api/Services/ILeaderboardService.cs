using GauntletLeaderboard.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Api.Services
{
    public interface ILeaderboardService
    {
        IEnumerable<LeaderboardGroup> GetLeaderboardGroups();
        IEnumerable<LeaderboardGroup> GetSubGroups(string groupName);
        IEnumerable<Leaderboard> GetLeaderboardsBySubGroup(string groupName, string subGroup);
        Leaderboard GetLeaderboard(int id, int page, int pageSize);
    }
}
