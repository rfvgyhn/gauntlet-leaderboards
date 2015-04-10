using GauntletLeaderboard.Core.Model;
using System.Collections.Generic;

namespace GauntletLeaderboard.Core.Services
{
    public interface ILeaderboardService
    {
        IEnumerable<Leaderboard> All();
        IEnumerable<Leaderboard> GetLeaderboardsByGroup(string groupName);
        IEnumerable<Leaderboard> GetLeaderboardsBySubGroup(string groupName, string subGroup);
        Leaderboard GetLeaderboard(int id);
        IPagedResult<Entry> GetLeaderboardEntries(int id, int page, int pageSize);
    }
}
