using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Web.Models.Home
{
    public class GroupsViewModel
    {
        public IEnumerable<Group> Groups { get; set; }
    }
}