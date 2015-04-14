using System.Web;
using System.Web.Optimization;

namespace GauntletLeaderboard.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;
            bundles.Add(new ScriptBundle("~/bundles/jquery", "https://code.jquery.com/jquery-2.1.3.min.js").Include(
                        "~/tmp/jquery.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js").Include(
                        "~/tmp/bootstrap.js"));
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
#if DEBUG
                        "~/tmp/datatables.js",
                        "~/tmp/datatables-bootstrap.js",
                        "~/Scripts/main.js"
#else
                        "~/dist/gauntlet-leaderboards.min.js"
#endif
            ));

            bundles.Add(new StyleBundle("~/Content/bootstrap", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css").Include(
                        "~/tmp/css/bootstrap.css"));
            bundles.Add(new StyleBundle("~/Content/font-awesome", "//maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css").Include(
                        "~/tmp/css/font-awesome.css"));
            bundles.Add(new StyleBundle("~/Content/app").Include(
#if DEBUG
                        "~/tmp/css/datatables-bootstrap.css",
                        "~/Content/gauntlet-leaderboards.bootstrap.theme.css",
                        "~/Content/Site.css"
#else
                        "~/dist/gauntlet-leaderboards.min.css"
#endif
                      ));
        }
    }
}
