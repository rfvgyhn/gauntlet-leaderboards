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

        public async Task<ActionResult> Details(long id)
        {
            var model = new DetailsViewModel
            {
                Player = await this.PlayerService.GetById(id)
            };

            return View(model);
        }
    }
}