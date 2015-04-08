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

        public Group GetByName(string groupName)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .GroupBy(l => l.Group)
                       .Where(g => g.Key == groupName)
                       .Select(g => new Group
                       {
                           Name = g.Key,
                           IsActive = g.Any(l => l.IsActive)
                       })
                       .Single();
        }

        public IEnumerable<SubGroup> GetSubGroups(string groupName)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.Group.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                       .GroupBy(l => l.SubGroup)
                       .Select(g => new SubGroup
                       {
                           Name = g.Key,
                           IsActive = g.Any(l => l.IsActive)
                       })
                       .ToArray();
        }
    }
}