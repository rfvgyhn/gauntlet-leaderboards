using System.Web;
using System.Web.Optimization;

namespace GauntletLeaderboard.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
#if DEBUG
                        "~/Scripts/jquery.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/main.js"
#else
                        "~/Scripts/gauntlet-leaderboards.min.js"
#endif
            ));

            bundles.Add(new StyleBundle("~/Content/app").Include(
#if DEBUG
                        "~/Content/css/bootstrap.css",
                        "~/Content/Site.css"
#else
                        "~/Scripts/gauntlet-leaderboards.min.js"
#endif
                      ));
        }
    }
}
