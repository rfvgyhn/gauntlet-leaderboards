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

            routes.MapRoute(
                name: "default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
