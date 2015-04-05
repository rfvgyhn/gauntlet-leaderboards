namespace GauntletLeaderboard.Api
{
    using GauntletLeaderboard.Api.Services;
    using Nancy;
    using Nancy.ModelBinding;

    public class LeaderboardModule : NancyModule
    {
        public LeaderboardModule(ILeaderboardService leaderboardService)
            : base("/leaderboards")
        {
            Get["/"] = parameters =>
            {
                var groups = leaderboardService.GetLeaderboardGroups();

                return Negotiate.WithModel(groups);
            };
            Get["/{group}"] = parameters =>
            {
                string group = parameters.group;
                var leaderboards = leaderboardService.GetSubGroups(group);

                return Negotiate.WithModel(leaderboards);
            };

            Get["/{group}/{subgroup}"] = parameters =>
            {
                string group = parameters.group;
                string subGroup = parameters.subGroup;
                var leaderboards = leaderboardService.GetLeaderboardsBySubGroup(group, subGroup);

                return Negotiate.WithModel(leaderboards);
            };

            Get["/{id:int}"] = parameters =>
            {
                int id = parameters.id;
                var query = this.Bind<Query>();
                var leaderboard = leaderboardService.GetLeaderboard(id, query.Page, query.PageSize);

                return Negotiate.WithModel(leaderboard);
            };
        }
    }
}