using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Core.Services
{
    public interface IGameService
    {
        Task<int> TotalCurrentlyPlaying();
    }
}
