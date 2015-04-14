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
        readonly IGroupService groupService;
        readonly ILeaderboardService leaderboardService;

        public HomeController(IGroupService groupService, ILeaderboardService leaderboardService)
        {
            this.groupService = groupService;
            this.leaderboardService = leaderboardService;
        }

        public ActionResult Groups()
        {
            var model = new GroupsViewModel
            {
                Groups = this.groupService.All()
            };

            return View(model);
        }

        public ActionResult SubGroups(string group)
        {
            var model = new SubGroupsViewModel
            {
                Groups = this.groupService.All(),
                Group = this.groupService.GetById(group),
                SubGroups = this.groupService.GetSubGroupsByGroup(group)
            };

            return View(model);
        }

        public ActionResult Leaderboards(string group, string subgroup)
        {
            var leaderboards = this.leaderboardService.GetLeaderboardsBySubGroup(group, subgroup);
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
                Groups = this.groupService.All(),
                Group = theGroup,
                Leaderboards = leaderboards,
                SubGroup = subGroup,
                SubGroups = this.groupService.GetSubGroupsByGroup(group)
            };

            return View(model);
        }

        public async Task<ActionResult> Leaderboard(string group, string subgroup, int leaderboardId, int? page = 1, int? pageSize = 20)
        {
            var model = new LeaderboardViewModel
            {
                Entries = await this.leaderboardService.GetLeaderboardEntries(leaderboardId, page.Value, pageSize.Value),
                Groups = this.groupService.All(),
                Leaderboard = this.leaderboardService.GetLeaderboard(leaderboardId),
                Leaderboards = this.leaderboardService.GetLeaderboardsBySubGroup(group, subgroup),
                SubGroups = this.groupService.GetSubGroupsByGroup(group)
            };

            return View(model);
        }
    }
}