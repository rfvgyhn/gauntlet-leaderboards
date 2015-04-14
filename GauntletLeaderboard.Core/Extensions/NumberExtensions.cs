using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Core.Extensions
{
    public static class NumberExtensions
    {
        public static TimeSpan ToTimeSpan(this int milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }
    }
}
