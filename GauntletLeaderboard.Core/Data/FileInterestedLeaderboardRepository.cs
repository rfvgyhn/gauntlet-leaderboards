using GauntletLeaderboard.Core.Extensions;
using GauntletLeaderboard.Core.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;

namespace GauntletLeaderboard.Core.Data
{
    public class FileInterestedLeaderboardRepository : IInterestedLeaderboardRepository
    {
        ObjectCache cache;
        string filePath;

        public FileInterestedLeaderboardRepository(string filePath, ObjectCache cache)
        {
            this.filePath = filePath;
            this.cache = cache;
        }

        public IEnumerable<InterestedLeaderboard> GetLeaderboards()
        {
            return this.cache.GetOrAdd(this.filePath, () => ParseFile());
        }

        private IEnumerable<InterestedLeaderboard> ParseFile()
        {
            var serializer = new JsonSerializer();

            using (var sr = File.OpenText(this.filePath))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<IEnumerable<InterestedLeaderboard>>(jsonTextReader);
            }
        }
    }
}