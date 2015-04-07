using GauntletLeaderboard.Api.Services;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GauntletLeaderboard.Api.Extensions;

namespace GauntletLeaderboard.Api
{
    public class GroupModule : NancyModule
    {
        public GroupModule(IGroupService groupService)
            : base("/groups")
        {
            Get["/"] = parameters =>
            {
                var result = groupService.All();

                return this.PrepareResult(result);
            };

            Get["/{group}"] = parameters =>
            {
                string group = parameters.group;
                var result = groupService.GetSubGroups(group);

                return this.PrepareResult(result);
            };
        }
    }
}