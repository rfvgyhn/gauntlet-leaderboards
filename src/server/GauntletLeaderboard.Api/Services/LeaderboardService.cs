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

        public Leaderboard GetLeaderboard(int id, int page, int pageSize)
        {
            Leaderboard leaderboard = null;
            SteamProfile[] profiles = null;
            var start = (page * pageSize) + 1;
            var end = (page * pageSize) + pageSize;
            var url = this.leaderboardUrl.Replace("{id}", id.ToString())
                                         .Replace("{start}", start.ToString())
                                         .Replace("{end}", end.ToString());

            using (var client = new WebClient())
            {
                
                leaderboard = ParseLeaderboardFromXml(client.DownloadString(url), id, page, pageSize);
                var steamIds = leaderboard.Entries.Page
                                          .Select(e => e.SteamId)
                                          .JoinWith(",");

                url = this.profileUrl.Replace("{steamids}", steamIds);
                var json = JObject.Parse(client.DownloadString(url));
                profiles = json["response"]["players"].ToObject<SteamProfile[]>();
            }

            var entries = leaderboard.Entries.Page.ToArray();
            foreach (var entry in entries)
                entry.SteamProfile = profiles.Where(p => p.SteamId == entry.SteamId).Single();

            leaderboard.Entries = entries.ToPagedResult(page, pageSize, leaderboard.Entries.TotalItemCount);
            return leaderboard;
        }

        private Leaderboard ParseLeaderboardFromXml(string xml, int id, int page, int pageSize)
        {
            var doc = XDocument.Parse(xml);
            var totalItems = int.Parse(doc.Root.Elements("totalLeaderboardEntries").Single().Value);
            var interestedLeaderboard = this.leaderboardRepository
                                            .GetLeaderboards()
                                            .Where(l => l.Id == id)
                                            .Single();
            var entries = doc.Root.Descendants("entry")
                                  .Deserialize<LeaderboardEntry>()
                                  .ToPagedResult(page, pageSize, totalItems);

            return new Leaderboard
            {
                Id = id,
                Name = interestedLeaderboard.Name,
                Entries = entries,
                IsActive = interestedLeaderboard.IsActive,
                Group = interestedLeaderboard.Group,
                SubGroup = interestedLeaderboard.SubGroup
            };
        }
    }
}