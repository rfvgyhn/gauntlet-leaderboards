using GauntletLeaderboard.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Core.Services
{
    public interface ILeaderboardService
    {
        IEnumerable<Leaderboard> All();
        IEnumerable<Leaderboard> GetLeaderboardsByGroup(string groupName);
        IEnumerable<Leaderboard> GetLeaderboardsBySubGroup(string groupName, string subGroup);
        Leaderboard GetLeaderboard(int id);
        Task<IPagedResult<Entry>> GetLeaderboardEntries(int id, int page, int pageSize);
        Task<IEnumerable<Entry>> GetLeaderboardEntriesForPlayer(long id);
    }
}
