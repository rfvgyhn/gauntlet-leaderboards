using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GauntletLeaderboard.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "players",
                url: "players",
                defaults: new { controller = "Players", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "player",
                url: "players/{id}",
                defaults: new { controller = "Players", action = "Details" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "search",
                url: "players/search/{id}",
                defaults: new { controller = "Players", action = "Search", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "player-by-name",
                url: "players/{name}",
                defaults: new { controller = "Players", action = "DetailsByName" }
            );

            routes.MapRoute(
                name: "home",
                url: "",
                defaults: new { controller = "Home", action = "Groups" }
            );

            routes.MapRoute(
                name: "subgroups",
                url: "{group}",
                defaults: new { controller = "Home", action = "SubGroups" }
            );

            routes.MapRoute(
                name: "leaderboards",
                url: "{group}/{subgroup}",
                defaults: new { controller = "Home", action = "Leaderboards" }
            );

            routes.MapRoute(
                name: "leaderboard",
                url: "{group}/{subgroup}/{leaderboardId}",
                defaults: new { controller = "Home", action = "Leaderboard" }
            );
        }
    }
}
