using GauntletLeaderboard.Api.Model;
using GauntletLeaderboard.Api.Services;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GauntletLeaderboard.Api.Extensions;

namespace GauntletLeaderboard.Api.Modules
{
    public class SubGroupModule : NancyModule
    {
        public SubGroupModule(IGroupService groupService)
            : base(ModuleRoute.SubGroup)
        {
            Func<SubGroup, string> leaderboardsLinkGenerator = m => ModuleRoute.Leaderboard + "/{0}/{1}".FormatWith(m.Group, m.Name);

            Get["/{name}"] = parameters =>
            {
                string group = parameters.name;
                var result = groupService.GetSubGroupsByName(group);

                return this.PrepareResult(result, leaderboardsLinkGenerator);
            };
        }
    }
}