using GauntletLeaderboard.Core.Services;
using GauntletLeaderboard.Web.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GauntletLeaderboard.Web.Controllers
{
    public class PlayersController : Controller
    {
        readonly IPlayerService PlayerService;

        public PlayersController(IPlayerService playerService)
        {
            this.PlayerService = playerService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> DetailsByName(string name)
        {
            var steamId = await this.PlayerService.ResolveVanityName(name);
            var model = await GetViewModel(steamId);

            return View("Details", model);
        }

        public async Task<ActionResult> Details(long id)
        {
            var model = await GetViewModel(id);

            return View(model);
        }

        private async Task<DetailsViewModel> GetViewModel(long id)
        {
            return new DetailsViewModel
            {
                Player = await this.PlayerService.GetById(id)
            };
        }
    }
}