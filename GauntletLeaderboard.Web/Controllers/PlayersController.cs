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

            if (steamId == null)
                return new HttpNotFoundResult("Player not found");

            var model = await GetViewModel(steamId.Value);

            return View("Details", model);
        }

        public async Task<ActionResult> Details(long id)
        {
            var model = await GetViewModel(id);

            if (model == null)
                return new HttpNotFoundResult("Player not found");

            return View(model);
        }

        private async Task<DetailsViewModel> GetViewModel(long id)
        {
            var player = await this.PlayerService.GetById(id);

            if (player == null)
                return null;

            return new DetailsViewModel
            {
                Player = player
            };
        }
    }
}