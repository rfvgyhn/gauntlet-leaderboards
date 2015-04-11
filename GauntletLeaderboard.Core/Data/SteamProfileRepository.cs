using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GauntletLeaderboard.Core.Extensions;
using System.Runtime.Caching;
using Newtonsoft.Json.Linq;
using System.Net;

namespace GauntletLeaderboard.Core.Data
{
    public class SteamProfileRepository : IProfileRepository
    {
        readonly string ProfileUrl;
        readonly string AchievementsUrl;
        readonly string BadgesUrl;
        readonly string VanityUrl;
        readonly int AppId;
        readonly ObjectCache Cache;
        readonly CacheItemPolicy CacheItemPolicy;

        public SteamProfileRepository(string steamApiKey, string profileUrl, string achievementsUrl, string badgesUrl, string vanityUrl, int appId, ObjectCache cache)
        {
            this.ProfileUrl = profileUrl.Replace("{key}", steamApiKey);
            this.AchievementsUrl = achievementsUrl.Replace("{key}", steamApiKey);
            this.BadgesUrl = badgesUrl.Replace("{key}", steamApiKey);
            this.VanityUrl = vanityUrl.Replace("{key}", steamApiKey);
            this.AppId = appId;
            this.Cache = cache;
            this.CacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5) };
        }

        public async Task<SteamProfile> GetById(long id)
        {
            var achievementsUrl = this.AchievementsUrl.Replace("{steamid}", id.ToString());
            var badgesUrl = this.BadgesUrl.Replace("{steamid}", id.ToString());
            var profile = (await GetByIds(new[] { id })).Single();

            using (var client = new WebClient())
            {
                try
                {
                    var badges = await client.DownloadStringTaskAsync(badgesUrl);
                    var achievements = await client.DownloadStringTaskAsync(achievementsUrl);
                    

                    var json = JObject.Parse(achievements);
                    var total = json["playerstats"]["achievements"].Count();
                    var achieved = json["playerstats"]["achievements"].Where(t => t.Value<int>("achieved") == 1).Count();
                    profile.AchievementPercentage = (float)achieved / total;

                    json = JObject.Parse(badges);
                    profile.Badges = json["response"]["badges"].ToObject<SteamBadge[]>().Where(b => b.AppId == this.AppId);
                }
                catch (WebException)
                {
                    // swallow
                }
            }

            return profile;
        }

        public async Task<IEnumerable<SteamProfile>> GetByIds(IEnumerable<long> ids)
        {
            var steamIdsParam = ids.JoinWith(",");
            var key = "steamProfiles:{0}".FormatWith(steamIdsParam);

            return await this.Cache.GetOrAdd(key, async () =>
            {
                using (var client = new WebClient())
                {
                    var profileUrl = this.ProfileUrl.Replace("{steamids}", steamIdsParam);
                    var response = await client.DownloadStringTaskAsync(profileUrl);
                    var json = JObject.Parse(response);

                    return json["response"]["players"].ToObject<SteamProfile[]>();
                }
            }, this.CacheItemPolicy);
        }

        public async Task<long> ResolveVanityName(string name)
        {
            var key = "vanityUrl:{0}".FormatWith(name);
            var cacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(5) };

            return await this.Cache.GetOrAdd(key, async () =>
            {
                using (var client = new WebClient())
                {
                    var url = this.VanityUrl.Replace("{name}", name);
                    var response = await client.DownloadStringTaskAsync(url);
                    var json = JObject.Parse(response);

                    return json["response"]["steamid"].ToObject<long>();
                }
            }, cacheItemPolicy);
        }
    }
}
