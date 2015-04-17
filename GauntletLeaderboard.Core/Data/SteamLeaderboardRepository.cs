using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GauntletLeaderboard.Core.Extensions;

namespace GauntletLeaderboard.Core.Data
{
    public class SteamLeaderboardRepository : ILeaderboardRepository
    {
        readonly string LeaderboardUrl;
        readonly string LeaderboardForProfileUrl;
        readonly IInterestedLeaderboardRepository InterestedLeaderboardRepository;

        public SteamLeaderboardRepository(string leaderboardUrl, string leaderboardForProfileUrl, IInterestedLeaderboardRepository interestedLeaderboardRepository)
        {
            this.LeaderboardUrl = leaderboardUrl;
            this.LeaderboardForProfileUrl = leaderboardForProfileUrl;
            this.InterestedLeaderboardRepository = interestedLeaderboardRepository;
        }

        public IEnumerable<Leaderboard> All()
        {
            return this.InterestedLeaderboardRepository
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
                       });
        }

        public IEnumerable<Leaderboard> Find(Func<Leaderboard, bool> predicate)
        {
            return this.InterestedLeaderboardRepository
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
                           Special = l.Special,
                           ScoreType = l.ScoreType
                       })
                       .Where(predicate);
        }

        public Leaderboard GetById(int id)
        {
            return this.InterestedLeaderboardRepository
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
                       .SingleOrDefault();
        }

        public async Task<IEnumerable<Entry>> GetLeaderboardEntriesForPlayer(long profileId)
        {
            var leaderboardIds = this.InterestedLeaderboardRepository.GetLeaderboards().Select(l => l.Id);
            var allEntries = new List<Task<Tuple<Entry[], int>>>();

            foreach (var id in leaderboardIds)
                allEntries.Add(GetLeaderboardEntriesForPlayer(id, profileId));

            await Task.WhenAll(allEntries.ToArray());
            var entries = allEntries.SelectMany(t => t.Result.Item1)
                                    .Where(e => e.SteamId == profileId);

            return entries;
        }

        private async Task<Tuple<Entry[], int>> GetLeaderboardEntriesForPlayer(int leaderboardId, long profileId)
        {
            Func<int, int, string> urlFactory = (s, e) =>
            {
                return this.LeaderboardForProfileUrl.Replace("{id}", leaderboardId.ToString())
                                                    .Replace("{start}", s.ToString())
                                                    .Replace("{end}", e.ToString())
                                                    .Replace("{steamid}", profileId.ToString());
            };

            return await Fetch(leaderboardId, urlFactory);
        }

        public async Task<Tuple<Entry[], int>> GetLeaderboardEntries(int leaderboardId, int page, int pageSize)
        {
            var start = ((page - 1) * pageSize) + 1;
            var end = ((page - 1) * pageSize) + pageSize;
            Func<int, int, string> urlFactory = (s, e) =>
            {
                return this.LeaderboardUrl.Replace("{id}", leaderboardId.ToString())
                                          .Replace("{start}", s.ToString())
                                          .Replace("{end}", e.ToString());
            };

            return await Fetch(leaderboardId, urlFactory, start, end);
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
                    entry.Leaderboard = GetById(id);
            }

            return new Tuple<Entry[], int>(entries, total);
        }
    }
}
