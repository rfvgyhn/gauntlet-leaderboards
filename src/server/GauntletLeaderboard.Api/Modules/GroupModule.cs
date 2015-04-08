using GauntletLeaderboard.Api.Services;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GauntletLeaderboard.Api.Extensions;
using GauntletLeaderboard.Api.Model;

namespace GauntletLeaderboard.Api.Modules
{
    public class GroupModule : NancyModule
    {
        public GroupModule(IGroupService groupService)
            : base("/groups")
        {
            Func<Group, string> subGroupsLinkGenerator = m => "/{name}/subgroups".Replace("{name}", m.Name);
            Get["/"] = parameters =>
            {
                var result = groupService.All();

                return this.PrepareResult(result, subGroupsLinkGenerator);
            };

            Get["/{name}"] = parameters =>
            {
                string group = parameters.name;
                var result = groupService.GetByName(group);

                return this.PrepareResult(result, subGroupsLinkGenerator);
            };

            Get["/{name}/subgroups"] = parameters =>
            {
                string group = parameters.name;
                var result = groupService.GetSubGroups(group);

                return this.PrepareResult<SubGroup>(result);
            };
        }
    }
}