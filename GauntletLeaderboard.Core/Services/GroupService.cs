using GauntletLeaderboard.Core.Data;
using GauntletLeaderboard.Core.Extensions;
using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GauntletLeaderboard.Core.Services
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
                       .OrderBy(g => g.Name)
                       .ToArray();
        }

        public Group GetById(string groupId)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .GroupBy(l => l.Group)
                       .Where(g => g.Key.ToSlug().Equals(groupId, StringComparison.OrdinalIgnoreCase))
                       .Select(g => new Group
                       {
                           Name = g.Key,
                           IsActive = g.Any(l => l.IsActive)
                       })
                       .OrderBy(g => g.Name)
                       .SingleOrDefault();
        }

        public IEnumerable<SubGroup> GetSubGroupsById(string subGroupId)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.SubGroup.ToSlug().Equals(subGroupId, StringComparison.OrdinalIgnoreCase))
                       .Select(l => new SubGroup
                       {
                           Group = new Group
                           {
                               Name = l.Group,
                               IsActive = l.IsActive
                           },
                           Name = l.SubGroup,
                           IsActive = l.IsActive
                       })
                       .GroupBy(l => l.Name)
                       .Select(g => g.First())
                       .OrderBy(g => g.Name)
                       .ToArray();
        }

        public IEnumerable<SubGroup> GetSubGroupsByGroup(string groupId)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.Group.ToSlug().Equals(groupId, StringComparison.OrdinalIgnoreCase))
                       .Select(l => new SubGroup
                       {
                           Group = new Group
                           {
                               Name = l.Group,
                               IsActive = l.IsActive
                           },
                           Name = l.SubGroup,
                           IsActive = l.IsActive
                       })
                       .GroupBy(l => l.Name)
                       .Select(g => g.First())
                       .OrderBy(g => g.Name)
                       .ToArray();
        }
    }
}