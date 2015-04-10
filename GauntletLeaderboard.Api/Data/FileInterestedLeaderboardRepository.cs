using GauntletLeaderboard.Api.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using GauntletLeaderboard.Api.Extensions;

namespace GauntletLeaderboard.Api.Data
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