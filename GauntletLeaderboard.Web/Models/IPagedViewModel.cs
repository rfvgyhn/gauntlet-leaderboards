using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Web.Models
{
    public interface IPagedViewModel
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalPages { get; }
        int TotalItems { get; }
        int? Next { get; }
        int? Previous { get; }
        int First { get; }
    }
}
