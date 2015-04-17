using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Core.Data
{
    public interface ILeaderboardRepository
    {
        IEnumerable<Leaderboard> All();
        IEnumerable<Leaderboard> Find(Func<Leaderboard, bool> predicate);
        Leaderboard GetById(int id);
        Task<IEnumerable<Entry>> GetLeaderboardEntriesForPlayer(long profileId);
        Task<Tuple<Entry[], int>> GetLeaderboardEntries(int leaderboardId, int page, int pageSize);
    }
}
