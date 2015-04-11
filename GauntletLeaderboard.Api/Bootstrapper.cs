using GauntletLeaderboard.Core.Data;
using GauntletLeaderboard.Core.Services;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System.IO;
using System.Runtime.Caching;
using System.Web.Configuration;

namespace GauntletLeaderboard.Api
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<ObjectCache, MemoryCache>(MemoryCache.Default);
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            var steamApiKey = WebConfigurationManager.AppSettings["steamApiKey"];
            var leaderboardUrl = WebConfigurationManager.AppSettings["leaderboardUrl"];
            var profileUrl = WebConfigurationManager.AppSettings["profileUrl"];
            var achievementsUrl = WebConfigurationManager.AppSettings["achievementsUrl"];
            var badgesUrl = WebConfigurationManager.AppSettings["badgesUrl"];
            var vanityUrl = WebConfigurationManager.AppSettings["vanityUrl"];
            var appId = int.Parse(WebConfigurationManager.AppSettings["appId"]);

            container.Register<IInterestedLeaderboardRepository>((c, p) => new FileInterestedLeaderboardRepository(Path.Combine(container.Resolve<IRootPathProvider>().GetRootPath(), "leaderboards.json"), c.Resolve<ObjectCache>()));
            container.Register<IGroupService, GroupService>();
            container.Register<IPlayerService, PlayerService>();
            container.Register<IProfileRepository>((c, p) => new SteamProfileRepository(steamApiKey, profileUrl, achievementsUrl, badgesUrl, vanityUrl, appId, c.Resolve<ObjectCache>()));
            container.Register<ILeaderboardService>((c, p) => new LeaderboardService(leaderboardUrl, c.Resolve<IInterestedLeaderboardRepository>(), c.Resolve<IProfileRepository>(), c.Resolve<ObjectCache>()));
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            pipelines.AfterRequest += (ctx) =>
            {
                var allowedOrigins = WebConfigurationManager.AppSettings["allowedOrigins"];
                ctx.Response.WithHeader("Access-Control-Allow-Origin", allowedOrigins)
                            .WithHeader("Access-Control-Allow-Methods", "GET OPTIONS")
                            .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");
            };
        }
    }
}