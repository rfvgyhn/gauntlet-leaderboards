using GauntletLeaderboard.Api.Extensions;
using GauntletLeaderboard.Core.Extensions;
using GauntletLeaderboard.Core.Model;
using GauntletLeaderboard.Core.Services;
using Nancy;
using System;

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
                var result = groupService.GetSubGroupsById(group);

                return this.PrepareResult(result, leaderboardsLinkGenerator);
            };
        }
    }
}