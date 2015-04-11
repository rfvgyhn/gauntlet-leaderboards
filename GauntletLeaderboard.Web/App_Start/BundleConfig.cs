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
                        "~/tmp/jquery.js",
                        "~/tmp/bootstrap.js",
                        "~/Scripts/main.js"
#else
                        "~/dist/gauntlet-leaderboards.min.js"
#endif
            ));

            bundles.Add(new StyleBundle("~/Content/app").Include(
#if DEBUG
                        "~/tmp/css/bootstrap.css",
                        "~/Content/Site.css"
#else
                        "~/dist/gauntlet-leaderboards.min.js"
#endif
                      ));
        }
    }
}
