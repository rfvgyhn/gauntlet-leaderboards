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
                           Group = new Group
                           {
                               Name = l.Group,
                               IsActive = l.IsActive
                           },
                           SubGroup = new SubGroup
                           {
                               Name = l.SubGroup,
                               IsActive = l.IsActive,
                               Group = new Group
                               {
                                   Name = l.Group,
                                   IsActive = l.IsActive
                               }
                           },
                           ScoreType = l.ScoreType
                       })
                       .OrderBy(g => g.Name)
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
                           Group = new Group
                           {
                               Name = l.Group,
                               IsActive = l.IsActive
                           },
                           SubGroup = new SubGroup
                           {
                               Name = l.SubGroup,
                               IsActive = l.IsActive,
                               Group = new Group
                               {
                                   Name = l.Group,
                                   IsActive = l.IsActive
                               }
                           },
                           Special = l.Special,
                           ScoreType = l.ScoreType
                       })
                       .OrderBy(g => g.Name)
                       .ToArray();
        }

        public IEnumerable<Leaderboard> GetLeaderboardsBySubGroup(string groupId, string subGroupId)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.Group.ToSlug().Equals(groupId, StringComparison.OrdinalIgnoreCase) &&
                                   l.SubGroup.ToSlug().Equals(subGroupId, StringComparison.OrdinalIgnoreCase))
                       .Select(l => new Leaderboard
                       {
                           Id = l.Id,
                           Name = l.Name,
                           IsActive = l.IsActive,
                           Group = new Group
                           {
                               Name = l.Group,
                               IsActive = l.IsActive
                           },
                           SubGroup = new SubGroup
                           {
                               Name = l.SubGroup,
                               IsActive = l.IsActive,
                               Group = new Group
                               {
                                   Name = l.Group,
                                   IsActive = l.IsActive
                               }
                           },
                           Special = l.Special,
                           ScoreType = l.ScoreType
                       })
                       .OrderBy(g => g.Name)
                       .ToArray();
        }

        public Leaderboard GetLeaderboard(int id)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.Id == id)
                       .Select(l => new Leaderboard
                       {
                           Id = l.Id,
                           IsActive = l.IsActive,
                           Name = l.Name,
                           Group = new Group
                           {
                               Name = l.Group,
                               IsActive = l.IsActive
                           },
                           SubGroup = new SubGroup
                           {
                               Name = l.SubGroup,
                               IsActive = l.IsActive,
                               Group = new Group
                               {
                                   Name = l.Group,
                                   IsActive = l.IsActive
                               }
                           },
                           Special = l.Special,
                           ScoreType = l.ScoreType
                       })
                       .Single();
        }

        public async Task<IPagedResult<Entry>> GetLeaderboardEntries(int id, int page, int pageSize)
        {
            var entriesKey = "GetLeaderboardEntries:entries:{0}:{1}:{2}".FormatWith(id, page, pageSize);
            var profilesKey = "GetLeaderboardEntries:profiles:{0}:{1}:{2}".FormatWith(id, page, pageSize);
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1) };
            var entryItems = await this.cache.GetOrAdd(entriesKey, async () => await FetchLeaderboardEntries(id, page, pageSize), cacheItemPolicy);
            var entries = entryItems.Item1;
            var totalItems = entryItems.Item2;
            var steamIds = entries.Select(e => e.SteamId);
            var profiles = await this.cache.GetOrAdd(profilesKey, async () => await this.profileRepository.GetByIds(steamIds), cacheItemPolicy);

            foreach (var entry in entries)
                entry.Player = profiles.Where(p => p.SteamId == entry.SteamId).Single();

            return entries.ToPagedResult(page, pageSize, totalItems);
        }

        public async Task<IEnumerable<Entry>> GetLeaderboardEntriesForPlayer(long id)
        {
            var key = "GetLeaderboardEntriesForPlayer:{0}".FormatWith(id);
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1) };
            var items = await this.cache.GetOrAdd(key, async () =>
            {
                var entriesTask = FetchLeaderboardEntries(id);
                var profileTask = this.profileRepository.GetById(id);

                await Task.WhenAll(entriesTask, profileTask);

                return new Tuple<IEnumerable<Entry>, SteamProfile>(entriesTask.Result, profileTask.Result);
            }, cacheItemPolicy);

            var entries = items.Item1;
            var profile = items.Item2;

            foreach (var entry in entries)
                entry.Player = profile;

            return entries;
        }

        private async Task<IEnumerable<Entry>> FetchLeaderboardEntries(long profileId)
        {
            var leaderboardIds = this.leaderboardRepository.GetLeaderboards().Select(l => l.Id);

            var allEntries = new List<Task<Tuple<Entry[], int>>>();

            foreach (var id in leaderboardIds)
                allEntries.Add(FetchLeaderboardEntries(id, profileId));

            await Task.WhenAll(allEntries.ToArray());
            var entries = allEntries.SelectMany(t => t.Result.Item1)
                                    .Where(e => e.SteamId == profileId);

            return entries;
        }

        private async Task<Tuple<Entry[], int>> FetchLeaderboardEntries(int id, long profileId)
        {
            Func<int, int, string> urlFactory = (s, e) =>
            {
                return this.leaderboardUrl.Replace("{id}", id.ToString())
                                         .Replace("{start}", s.ToString())
                                         .Replace("{end}", e.ToString()) + "&steamid=" + profileId.ToString();
            };

            return await Fetch(id, urlFactory);
        }

        private async Task<Tuple<Entry[], int>> FetchLeaderboardEntries(int id, int page, int pageSize)
        {
            var start = ((page - 1) * pageSize) + 1;
            var end = ((page - 1) * pageSize) + pageSize;
            Func<int, int, string> urlFactory = (s, e) =>
            {
                return this.leaderboardUrl.Replace("{id}", id.ToString())
                                          .Replace("{start}", s.ToString())
                                          .Replace("{end}", e.ToString());
            };

            return await Fetch(id, urlFactory, start, end);
        }

        private async Task<Tuple<Entry[], int>> Fetch(int id, Func<int, int, string> urlFactory, int start = 0, int end = 5000)
        {
            var total = 0;
            Entry[] entries;

            using (var client = new HttpClient())
            {
                var url = urlFactory(start, end);
                var stats = await client.GetStringAsync(url);
                var doc = XDocument.Parse(stats);

                total = int.Parse(doc.Root.Elements("totalLeaderboardEntries").Single().Value);
                    
                entries = doc.Root.Descendants("entry")
                                .Deserialize<Entry>()
                                .OrderBy(e => e.Rank)
                                .ToArray();

                foreach (var entry in entries)
                    entry.Leaderboard = GetLeaderboard(id);
            }

            return new Tuple<Entry[], int>(entries, total);
        }
    }
}