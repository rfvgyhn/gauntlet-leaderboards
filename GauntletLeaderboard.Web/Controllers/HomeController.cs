using GauntletLeaderboard.Core.Model;
using GauntletLeaderboard.Core.Services;
using GauntletLeaderboard.Web.Models;
using GauntletLeaderboard.Web.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GauntletLeaderboard.Web.Controllers
{
    public class HomeController : Controller
    {
        readonly IGroupService GroupService;
        readonly ILeaderboardService LeaderboardService;
        readonly IGameService GameService;

        public HomeController(IGroupService groupService, ILeaderboardService leaderboardService, IGameService gameService)
        {
            this.GroupService = groupService;
            this.LeaderboardService = leaderboardService;
            this.GameService = gameService;
        }

        public async Task<JsonResult> TotalCurrentlyPlaying()
        {
            var total = await this.GameService.TotalCurrentlyPlaying();

            return Json(total, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Groups()
        {
            var model = new GroupsViewModel
            {
                Groups = this.GroupService.All()
            };

            return View(model);
        }

        public ActionResult SubGroups(string group)
        {
            var theGroup = this.GroupService.GetById(group);

            if (theGroup == null)
                return new HttpNotFoundResult("Group not found");

            var model = new SubGroupsViewModel
            {
                Groups = this.GroupService.All(),
                Group = theGroup,
                SubGroups = this.GroupService.GetSubGroupsByGroup(group)
            };

            return View(model);
        }

        public ActionResult Leaderboards(string group, string subgroup)
        {
            var leaderboards = this.LeaderboardService.GetLeaderboardsBySubGroup(group, subgroup);
            Group theGroup = null;
            SubGroup subGroup = null;

            if (leaderboards.Any())
            {
                var leadboard = leaderboards.First();
                theGroup = leadboard.Group;
                subGroup = leadboard.SubGroup;
            }

            var model = new LeaderboardsViewModel
            {
                Groups = this.GroupService.All(),
                Group = theGroup,
                Leaderboards = leaderboards,
                SubGroup = subGroup,
                SubGroups = this.GroupService.GetSubGroupsByGroup(group)
            };

            return View(model);
        }

        public async Task<ActionResult> Leaderboard(string group, string subgroup, int leaderboardId, int? page = 1, int? pageSize = 20)
        {
            var leaderboard = this.LeaderboardService.GetLeaderboard(leaderboardId);

            if (leaderboard == null)
                return new HttpNotFoundResult("Leaderboard not found");

            var model = new LeaderboardViewModel
            {
                Entries = await this.LeaderboardService.GetLeaderboardEntries(leaderboardId, page.Value, pageSize.Value),
                Groups = this.GroupService.All(),
                Leaderboard = leaderboard,
                Leaderboards = this.LeaderboardService.GetLeaderboardsBySubGroup(group, subgroup),
                SubGroups = this.GroupService.GetSubGroupsByGroup(group)
            };

            return View(model);
        }
    }
}