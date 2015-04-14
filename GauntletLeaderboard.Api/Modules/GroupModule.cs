
using GauntletLeaderboard.Api.Extensions;
using GauntletLeaderboard.Core.Extensions;
using GauntletLeaderboard.Core.Model;
using GauntletLeaderboard.Core.Services;
using Nancy;
using System;

namespace GauntletLeaderboard.Api.Modules
{
    public class GroupModule : NancyModule
    {
        public GroupModule(IGroupService groupService)
            : base(ModuleRoute.Group)
        {
            Func<Group, string> subGroupsLinkGenerator = m => "{0}/subgroups".FormatWith(m.Name);
            Func<SubGroup, string> leaderboardsLinkGenerator = m => ModuleRoute.Leaderboard + "/{0}/{1}".FormatWith(m.Group, m.Name);

            Get["/"] = parameters =>
            {
                var result = groupService.All();

                return this.PrepareResult(result, subGroupsLinkGenerator);
            };

            Get["/{name}"] = parameters =>
            {
                string group = parameters.name;
                var result = groupService.GetById(group);

                return this.PrepareResult(result, subGroupsLinkGenerator);
            };

            Get["/{name}/subgroups"] = parameters =>
            {
                string group = parameters.name;
                var result = groupService.GetSubGroupsByGroup(group);

                return this.PrepareResult(result, leaderboardsLinkGenerator);
            };
        }
    }
}