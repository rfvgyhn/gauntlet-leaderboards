using GauntletLeaderboard.Core.Model;
using System.Collections.Generic;

namespace GauntletLeaderboard.Core.Services
{
    public interface IGroupService
    {
        IEnumerable<Group> All();
        Group GetByName(string groupName);
        IEnumerable<SubGroup> GetSubGroupsByName(string subGroupName);
        IEnumerable<SubGroup> GetSubGroupsByGroup(string groupName);
    }
}
