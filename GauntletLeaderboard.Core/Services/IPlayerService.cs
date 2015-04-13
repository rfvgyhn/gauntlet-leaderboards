using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Core.Services
{
    public interface IPlayerService
    {
        IPagedResult<Player> All(int page, int pageSize);
        Task<Player> GetById(long id);
        Task<long?> ResolveVanityName(string name);
    }
}
