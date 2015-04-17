using GauntletLeaderboard.Core.Data;
using GauntletLeaderboard.Core.Extensions;
using GauntletLeaderboard.Core.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GauntletLeaderboard.Core.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        readonly ILeaderboardRepository LeaderboardRepository;
        
        readonly IProfileRepository ProfileRepository;
        readonly ObjectCache Cache;

        public LeaderboardService(ILeaderboardRepository leaderboardRepository, IProfileRepository profileRepository, ObjectCache cache)
        {
            this.LeaderboardRepository = leaderboardRepository;
            this.ProfileRepository = profileRepository;
            this.Cache = cache;
        }

        public IEnumerable<Leaderboard> All()
        {
            return this.LeaderboardRepository
                       .All()
                       .OrderBy(g => g.Name)
                       .ToArray();
        }

        public IEnumerable<Leaderboard> GetLeaderboardsByGroup(string groupName)
        {
            return this.LeaderboardRepository
                       .Find(l => l.Group.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                       .OrderBy(g => g.Name)
                       .ToArray();
        }

        public IEnumerable<Leaderboard> GetLeaderboardsBySubGroup(string groupId, string subGroupId)
        {
            return this.LeaderboardRepository
                       .Find(l => l.Group.Id.Equals(groupId, StringComparison.OrdinalIgnoreCase) &&
                                  l.SubGroup.Id.Equals(subGroupId, StringComparison.OrdinalIgnoreCase))
                       .OrderBy(g => g.Name)
                       .ToArray();
        }

        public Leaderboard GetLeaderboard(int id)
        {
            return this.LeaderboardRepository.GetById(id);
        }

        public async Task<IPagedResult<Entry>> GetLeaderboardEntries(int id, int page, int pageSize)
        {
            var entriesKey = "GetLeaderboardEntries:entries:{0}:{1}:{2}".FormatWith(id, page, pageSize);
            var profilesKey = "GetLeaderboardEntries:profiles:{0}:{1}:{2}".FormatWith(id, page, pageSize);
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1) };
            var entryItems = await this.Cache.GetOrAdd(entriesKey, async () => await this.LeaderboardRepository.GetLeaderboardEntries(id, page, pageSize), cacheItemPolicy);
            var entries = entryItems.Item1;
            var totalItems = entryItems.Item2;
            var steamIds = entries.Select(e => e.SteamId);
            var profiles = await this.Cache.GetOrAdd(profilesKey, async () => await this.ProfileRepository.GetByIds(steamIds), cacheItemPolicy);

            foreach (var entry in entries)
                entry.Player = profiles.Where(p => p.SteamId == entry.SteamId).Single();

            return entries.ToPagedResult(page, pageSize, totalItems);
        }

        public async Task<IEnumerable<Entry>> GetLeaderboardEntriesForPlayer(long id)
        {
            var key = "GetLeaderboardEntriesForPlayer:{0}".FormatWith(id);
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1) };
            var items = await this.Cache.GetOrAdd(key, async () =>
            {
                var entriesTask = this.LeaderboardRepository.GetLeaderboardEntriesForPlayer(id);
                var profileTask = this.ProfileRepository.GetById(id);

                await Task.WhenAll(entriesTask, profileTask);

                return new Tuple<IEnumerable<Entry>, SteamProfile>(entriesTask.Result, profileTask.Result);
            }, cacheItemPolicy);

            var entries = items.Item1;
            var profile = items.Item2;

            foreach (var entry in entries)
                entry.Player = profile;

            return entries;
        }
    }
}