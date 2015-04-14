using GauntletLeaderboard.Core.Model;
using System.Collections.Generic;

namespace GauntletLeaderboard.Core.Services
{
    public interface IGroupService
    {
        IEnumerable<Group> All();
        Group GetById(string groupName);
        IEnumerable<SubGroup> GetSubGroupsById(string subGroupName);
        IEnumerable<SubGroup> GetSubGroupsByGroup(string groupName);
    }
}
