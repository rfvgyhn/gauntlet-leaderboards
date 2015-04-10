
namespace GauntletLeaderboard.Core.Model
{
    public class InterestedLeaderboard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string SubGroup { get; set; }
        public string Special { get; set; }
        public bool IsActive { get; set; }
    }
}