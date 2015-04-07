using GauntletLeaderboard.Api.Data;
using GauntletLeaderboard.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Api.Services
{
    public class GroupService : IGroupService
    {
        readonly IInterestedLeaderboardRepository leaderboardRepository;

        public GroupService(IInterestedLeaderboardRepository leaderboardRepository)
        {
            this.leaderboardRepository = leaderboardRepository;
        }

        public IEnumerable<Group> All()
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .GroupBy(l => l.Group)
                       .Select(g => new Group
                       {
                           Name = g.Key,
                           IsActive = g.Any(l => l.IsActive)
                       })
                       .ToArray();
        }

        public IEnumerable<Group> GetSubGroups(string groupName)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.Group.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                       .GroupBy(l => l.SubGroup)
                       .Select(g => new Group
                       {
                           Name = g.Key,
                           IsActive = g.Any(l => l.IsActive)
                       })
                       .ToArray();
        }
    }
}