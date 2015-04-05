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

        public IEnumerable<LeaderboardGroup> GetLeaderboardGroups()
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .GroupBy(l => l.Group)
                       .Select(g => new LeaderboardGroup
                       {
                           Name = g.Key,
                           IsActive = g.Any(l => l.IsActive)
                       });
        }

        public IEnumerable<LeaderboardGroup> GetSubGroups(string groupName)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.Group.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                       .GroupBy(l => l.SubGroup)
                       .Select(g => new LeaderboardGroup
                       {
                           Name = g.Key,
                           IsActive = g.Any(l => l.IsActive)
                       });
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
                           IsActive = l.IsActive
                       });
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

        public IPagedResult<LeaderboardEntry> GetLeaderboardEntries(int id, int page, int pageSize)
        {
            var entries = Enumerable.Empty<LeaderboardEntry>();
            SteamProfile[] profiles = null;
            var start = (page * pageSize) + 1;
            var end = (page * pageSize) + pageSize;
            var url = this.leaderboardUrl.Replace("{id}", id.ToString())
                                         .Replace("{start}", start.ToString())
                                         .Replace("{end}", end.ToString());
            int totalItems;

            using (var client = new WebClient())
            {   
                entries = ParseLeaderboardEntiresFromXml(client.DownloadString(url), id, page, pageSize, out totalItems);
                var steamIds = entries.Select(e => e.SteamId)
                                      .JoinWith(",");

                url = this.profileUrl.Replace("{steamids}", steamIds);
                var json = JObject.Parse(client.DownloadString(url));
                profiles = json["response"]["players"].ToObject<SteamProfile[]>();
            }

            foreach (var entry in entries)
                entry.SteamProfile = profiles.Where(p => p.SteamId == entry.SteamId).Single();

            return entries.ToPagedResult(page, pageSize, 0);
        }

        private IEnumerable<LeaderboardEntry> ParseLeaderboardEntiresFromXml(string xml, int id, int page, int pageSize, out int totalItems)
        {
            var doc = XDocument.Parse(xml);
            totalItems = int.Parse(doc.Root.Elements("totalLeaderboardEntries").Single().Value);

            return doc.Root.Descendants("entry")
                           .Deserialize<LeaderboardEntry>()
                           .ToArray();
        }
    }
}