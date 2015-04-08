using GauntletLeaderboard.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Api.Services
{
    public interface IGroupService
    {
        IEnumerable<Group> All();
        Group GetByName(string groupName);
        IEnumerable<SubGroup> GetSubGroups(string groupName);
    }
}
