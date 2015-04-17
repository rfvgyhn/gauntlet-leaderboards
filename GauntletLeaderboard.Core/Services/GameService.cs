using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using GauntletLeaderboard.Core.Extensions;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace GauntletLeaderboard.Core.Services
{
    public class GameService : IGameService
    {
        readonly string CurrentPlayersUrl;
        readonly ObjectCache Cache;

        public GameService(string steamApiKey, string currentPlayersUrl, ObjectCache cache)
        {
            this.CurrentPlayersUrl = currentPlayersUrl.Replace("{key}", steamApiKey);
            this.Cache = cache;
        }

        public async Task<int> TotalCurrentlyPlaying()
        {
            var key = "TotalCurrentlyPlaying";
            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1) };

            return await this.Cache.GetOrAdd(key, async () =>
            {
                using (var client = new HttpClient())
                {
                    var stats = await client.GetStringAsync(this.CurrentPlayersUrl);
                    var json = JObject.Parse(stats);

                    if (json["response"]["player_count"] != null)
                        return json["response"]["player_count"].ToObject<int>();

                    return 0;
                }
            }, policy);
        }
    }
}
