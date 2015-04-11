using GauntletLeaderboard.Core.Services;
using GauntletLeaderboard.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Group = this.groupService.GetByName(group),
                SubGroups = this.groupService.GetSubGroupsByGroup(group)
            };

            return View(model);
        }

        public ActionResult Leaderboards(string group, string subgroup)
        {
            var leaderboards = this.leaderboardService.GetLeaderboardsBySubGroup(group, subgroup);
            string groupName = null;
            string subGroupName = null;

            if (leaderboards.Any())
            {
                var leadboard = leaderboards.First();
                groupName = leadboard.Group;
                subGroupName = leadboard.SubGroup;
            }

            var model = new LeaderboardsViewModel
            {
                Groups = this.groupService.All(),
                Group = groupName,
                Leaderboards = leaderboards,
                SubGroup = subGroupName,
                SubGroups = this.groupService.GetSubGroupsByGroup(group)
            };

            return View(model);
        }

        public ActionResult Leaderboard(string group, string subgroup, int leaderboardId, int? page = 1, int? pageSize = 20)
        {
            var leaderboards = this.leaderboardService.GetLeaderboardsBySubGroup(group, subgroup);
            string groupName = null;
            string subGroupName = null;

            if (leaderboards.Any())
            {
                var leadboard = leaderboards.First();
                groupName = leadboard.Group;
                subGroupName = leadboard.SubGroup;
            }

            var model = new LeaderboardViewModel
            {
                Entries = this.leaderboardService.GetLeaderboardEntries(leaderboardId, page.Value, pageSize.Value),
                Groups = this.groupService.All(),
                Leaderboard = this.leaderboardService.GetLeaderboard(leaderboardId),
                Leaderboards = leaderboards,
                SubGroups = this.groupService.GetSubGroupsByGroup(group)
            };

            return View(model);
        }
    }
}