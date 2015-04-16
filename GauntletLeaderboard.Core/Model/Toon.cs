using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletLeaderboard.Core.Model
{
    [Flags]
    public enum Toon
    {
        Warrior = 1,
        Valkryie = 2,
        Wizard = 4,
        Elf = 8,
        Necromancer = 16
    }
}
