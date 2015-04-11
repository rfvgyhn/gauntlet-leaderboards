using GauntletLeaderboard.Core.Data;
using GauntletLeaderboard.Core.Extensions;
using GauntletLeaderboard.Core.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GauntletLeaderboard.Core.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        readonly string leaderboardUrl;
        readonly IInterestedLeaderboardRepository leaderboardRepository;
        readonly IProfileRepository profileRepository;
        readonly ObjectCache cache;

        public LeaderboardService(string leaderboardUrl, IInterestedLeaderboardRepository leaderboardRepository, IProfileRepository profileRepository, ObjectCache cache)
        {
            this.leaderboardUrl = leaderboardUrl;
            this.leaderboardRepository = leaderboardRepository;
            this.profileRepository = profileRepository;
            this.cache = cache;
        }

        public IEnumerable<Leaderboard> All()
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Select(l => new Leaderboard
                       {
                           Id = l.Id,
                           Name = l.Name,
                           IsActive = l.IsActive,
                           Group = l.Group,
                           SubGroup = l.SubGroup
                       })
                       .ToArray();
        }

        public IEnumerable<Leaderboard> GetLeaderboardsByGroup(string groupName)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.Group.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                       .Select(l => new Leaderboard
                       {
                           Id = l.Id,
                           Name = l.Name,
                           IsActive = l.IsActive,
                           Group = l.Group,
                           SubGroup = l.SubGroup,
                           Special = l.Special
                       })
                       .ToArray();
        }

        public IEnumerable<Leaderboard> GetLeaderboardsBySubGroup(string groupName, string subGroup)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.Group.Equals(groupName, StringComparison.OrdinalIgnoreCase) &&
                                   l.SubGroup.Equals(subGroup, StringComparison.OrdinalIgnoreCase))
                       .Select(l => new Leaderboard
                       {
                           Id = l.Id,
                           Name = l.Name,
                           IsActive = l.IsActive,
                           Group = l.Group,
                           SubGroup = l.SubGroup,
                           Special = l.Special
                       })
                       .ToArray();
        }

        public Leaderboard GetLeaderboard(int id)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.Id == id)
                       .Select(l => new Leaderboard
                       {
                           Group = l.Group,
                           Id = l.Id,
                           IsActive = l.IsActive,
                           Name = l.Name,
                           SubGroup = l.SubGroup,
                           Special = l.Special
                       })
                       .Single();
        }

        public async Task<IPagedResult<Entry>> GetLeaderboardEntries(int id, int page, int pageSize)
        {
            var entryItems = await FetchLeaderboardEntries(id, page, pageSize);
            var entries = entryItems.Item1;
            var totalItems = entryItems.Item2;
            var steamIds = entries.Select(e => e.SteamId);
            var profiles = await this.profileRepository.GetByIds(steamIds);

            foreach (var entry in entries)
                entry.Player = profiles.Where(p => p.SteamId == entry.SteamId).Single();

            return entries.ToPagedResult(page, pageSize, totalItems);
        }

        public async Task<IEnumerable<Entry>> GetLeaderboardEntriesForPlayer(long id)
        {
            var entries = await FetchLeaderboardEntries(id);
            var profile = await this.profileRepository.GetById(id);

            foreach (var entry in entries)
                entry.Player = profile;

            return entries;
        }

        private async Task<IEnumerable<Entry>> FetchLeaderboardEntries(long profileId)
        {
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1) };
            var leaderboardIds = this.leaderboardRepository.GetLeaderboards().Select(l => l.Id);

            var allEntries = new List<Task<Tuple<Entry[], int>>>();

            foreach (var id in leaderboardIds)
                allEntries.Add(FetchLeaderboardEntries(id, 1, int.MaxValue));

            await Task.WhenAll(allEntries.ToArray());
            var entries = allEntries.SelectMany(t => t.Result.Item1)
                                    .Where(e => e.SteamId == profileId);

            return entries;
        }

        private async Task<Tuple<Entry[], int>> FetchLeaderboardEntries(int id, int page, int pageSize)
        {
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1) };
            var start = ((page - 1) * pageSize) + 1;
            var end = ((page - 1) * pageSize) + pageSize;
            var url = this.leaderboardUrl.Replace("{id}", id.ToString())
                                         .Replace("{start}", start.ToString())
                                         .Replace("{end}", end.ToString());
            var total = 0;
            var key = "leaderboardEntires:{0}:{1}:{2}".FormatWith(id, page, pageSize);
            var entries = await this.cache.GetOrAdd(key, async () =>
            {
                using (var client = new WebClient())
                {
                    var stats = await client.DownloadStringTaskAsync(url);
                    var doc = XDocument.Parse(stats);

                    total = int.Parse(doc.Root.Elements("totalLeaderboardEntries").Single().Value);
                    var items = doc.Root.Descendants("entry")
                                   .Deserialize<Entry>()
                                   .OrderBy(e => e.Rank)
                                   .ToArray();

                    foreach (var item in items)
                        item.Leaderboard = GetLeaderboard(id);

                    return items;
                }
            }, cacheItemPolicy);

            return new Tuple<Entry[], int>(entries, total);
        }
    }
}