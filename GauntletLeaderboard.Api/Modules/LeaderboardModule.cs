using GauntletLeaderboard.Api.Extensions;
using GauntletLeaderboard.Core.Extensions;
using GauntletLeaderboard.Core.Model;
using GauntletLeaderboard.Core.Services;
using Nancy;
using Nancy.ModelBinding;
using System;

namespace GauntletLeaderboard.Api.Modules
{
    public class LeaderboardModule : NancyModule
    {
        public LeaderboardModule(ILeaderboardService leaderboardService)
            : base(ModuleRoute.Leaderboard)
        {
            Func<Leaderboard, string> entriesLinkGenerator = m => "{0}/entries".FormatWith(m.Id);

            Get["/"] = parameters =>
            {
                var result = leaderboardService.All();

                return this.PrepareResult(result, entriesLinkGenerator);
            };

            Get["/{group}"] = parameters =>
            {
                string group = parameters.group;
                var result = leaderboardService.GetLeaderboardsByGroup(group);

                return this.PrepareResult(result, entriesLinkGenerator);
            };

            Get["/{group}/{subgroup}"] = parameters =>
            {
                string group = parameters.group;
                string subGroup = parameters.subGroup;
                var result = leaderboardService.GetLeaderboardsBySubGroup(group, subGroup);

                return this.PrepareResult(result, entriesLinkGenerator);
            };

            Get["/{id:int}"] = parameters =>
            {
                int id = parameters.id;
                var result = leaderboardService.GetLeaderboard(id);

                return this.PrepareResult(result, entriesLinkGenerator);
            };

            Get["/{id:int}/entries", true] = async (parameters, ct) =>
            {
                int id = parameters.id;
                var query = this.Bind<Query>();
                var result = await leaderboardService.GetLeaderboardEntries(id, query.Page.HasValue ? query.Page.Value : 1, query.PageSize.HasValue ? query.PageSize.Value : 20);
                
                return this.PrepareResult(result, query);
            };
        }
    }
}