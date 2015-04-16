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
using System.Net.Http;

namespace GauntletLeaderboard.Core.Data
{
    public class SteamProfileRepository : IProfileRepository
    {
        readonly string ProfileUrl;
        readonly string AchievementsUrl;
        readonly string BadgesUrl;
        readonly string VanityUrl;
        readonly int AppId;

        public SteamProfileRepository(string steamApiKey, string profileUrl, string achievementsUrl, string badgesUrl, string vanityUrl, int appId)
        {
            this.ProfileUrl = profileUrl.Replace("{key}", steamApiKey);
            this.AchievementsUrl = achievementsUrl.Replace("{key}", steamApiKey);
            this.BadgesUrl = badgesUrl.Replace("{key}", steamApiKey);
            this.VanityUrl = vanityUrl.Replace("{key}", steamApiKey);
            this.AppId = appId;
        }

        public async Task<SteamProfile> GetById(long id)
        {
            var achievementsUrl = this.AchievementsUrl.Replace("{steamid}", id.ToString());
            var badgesUrl = this.BadgesUrl.Replace("{steamid}", id.ToString());
            var profile = (await GetByIds(new[] { id })).SingleOrDefault();

            if (profile == null)
                return null;

            using (var client = new HttpClient())
            {
                try
                {
                    var badges = await client.GetStringAsync(badgesUrl);
                    var achievements = await client.GetStringAsync(achievementsUrl);
                    

                    var json = JObject.Parse(achievements);
                    var total = json["playerstats"]["achievements"].Count();
                    var achieved = json["playerstats"]["achievements"].Where(t => t.Value<int>("achieved") == 1).Count();
                    profile.AchievementPercentage = (float)achieved / total;

                    json = JObject.Parse(badges);

                    if (json["response"]["badges"] != null)
                        profile.Badges = json["response"]["badges"].ToObject<SteamBadge[]>().Where(b => b.AppId == this.AppId);
                    else
                        profile.Badges = Enumerable.Empty<SteamBadge>();
                }
                catch (HttpRequestException)
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

            using (var client = new HttpClient())
            {
                var profileUrl = this.ProfileUrl.Replace("{steamids}", steamIdsParam);
                var response = await client.GetStringAsync(profileUrl);
                var json = JObject.Parse(response);
                    
                return json["response"]["players"].ToObject<SteamProfile[]>();
            }
        }

        public async Task<long?> ResolveVanityName(string name)
        {
            var key = "vanityUrl:{0}".FormatWith(name);
            var cacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(5) };

            using (var client = new HttpClient())
            {
                var url = this.VanityUrl.Replace("{name}", name);
                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);
                var steamId = json["response"]["steamid"];

                if (steamId == null)
                    return null;

                return steamId.ToObject<long?>();
            }
        }
    }
}
