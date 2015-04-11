using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Core.Data
{
    public interface IProfileRepository
    {
        Task<SteamProfile> GetById(long id);
        Task<IEnumerable<SteamProfile>> GetByIds(IEnumerable<long> ids);
    }
}
