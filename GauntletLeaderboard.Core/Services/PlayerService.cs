using GauntletLeaderboard.Core.Data;
using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Core.Services
{
    public class PlayerService : IPlayerService
    {
        readonly IProfileRepository ProfileRepository;
        readonly ILeaderboardService LeaderboardService;

        public PlayerService(IProfileRepository profileRepository, ILeaderboardService leaderboardService)
        {
            this.ProfileRepository = profileRepository;
            this.LeaderboardService = leaderboardService;
        }

        public IPagedResult<Player> All(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<Player> GetById(long id)
        {
            var profile = await this.ProfileRepository.GetById(id);

            if (profile == null)
                return null;

            return new Player
            {
                Entries = await this.LeaderboardService.GetLeaderboardEntriesForPlayer(id),
                Profile = profile
            };
        }

        public async Task<long?> ResolveVanityName(string name)
        {
            return await this.ProfileRepository.ResolveVanityName(name);
        }
    }
}
