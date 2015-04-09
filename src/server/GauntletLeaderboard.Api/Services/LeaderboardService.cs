using GauntletLeaderboard.Api.Data;
using GauntletLeaderboard.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Web;
using GauntletLeaderboard.Api.Extensions;
using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace GauntletLeaderboard.Api.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        readonly string profileUrl;
        readonly string leaderboardUrl;
        readonly IInterestedLeaderboardRepository leaderboardRepository;
        readonly ObjectCache cache;

        public LeaderboardService(string steamApiKey, string profileUrl, string leaderboardUrl, IInterestedLeaderboardRepository leaderboardRepository, ObjectCache cache)
        {
            this.profileUrl = profileUrl.Replace("{key}", steamApiKey);
            this.leaderboardUrl = leaderboardUrl;
            this.leaderboardRepository = leaderboardRepository;
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
                           SubGroup = l.SubGroup
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
                           SubGroup = l.SubGroup
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
                           SubGroup = l.SubGroup
                       })
                       .Single();
        }

        public IPagedResult<Entry> GetLeaderboardEntries(int id, int page, int pageSize)
        {
            var entries = Enumerable.Empty<Entry>();
            IEnumerable<SteamProfile> profiles = null;
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1) };
            int totalItems;

            using (var client = new WebClient())
            {   
                entries = FetchLeaderboardEntries(client, id, page, pageSize, cacheItemPolicy, out totalItems);
                var steamIds = entries.Select(e => e.SteamId);
                profiles = FetchSteamProfiles(client, steamIds, cacheItemPolicy);
            }

            foreach (var entry in entries)
                entry.Player = profiles.Where(p => p.SteamId == entry.SteamId).Single();

            return entries.ToPagedResult(page, pageSize, totalItems);
        }

        private IEnumerable<Entry> FetchLeaderboardEntries(WebClient client, int id, int page, int pageSize, CacheItemPolicy cacheItemPolicy, out int totalItems)
        {
            var start = ((page - 1) * pageSize) + 1;
            var end = ((page - 1) * pageSize) + pageSize;
            var url = this.leaderboardUrl.Replace("{id}", id.ToString())
                                         .Replace("{start}", start.ToString())
                                         .Replace("{end}", end.ToString());
            var total = 0;
            var key = "leaderboardEntires:{0}:{1}:{2}".FormatWith(id, page, pageSize);
            var entries = this.cache.GetOrAdd(key, () =>
            {
                var doc = XDocument.Parse(client.DownloadString(url));
                total = int.Parse(doc.Root.Elements("totalLeaderboardEntries").Single().Value);

                return doc.Root.Descendants("entry")
                               .Deserialize<Entry>()
                               .OrderBy(e => e.Rank)
                               .ToArray();
            }, cacheItemPolicy);
            totalItems = total;

            return entries;
        }

        private IEnumerable<SteamProfile> FetchSteamProfiles(WebClient client, IEnumerable<string> steamIds, CacheItemPolicy cacheItemPolicy)
        {
            var steamIdsParam = steamIds.JoinWith(",");
            var key = "steamProfiles:{0}".FormatWith(steamIdsParam);

            return this.cache.GetOrAdd(key, () =>
            {
                var url = this.profileUrl.Replace("{steamids}", steamIdsParam);
                var json = JObject.Parse(client.DownloadString(url));

                return json["response"]["players"].ToObject<SteamProfile[]>();
            }, cacheItemPolicy);
        }
    }
}