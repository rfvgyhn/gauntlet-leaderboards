namespace GauntletLeaderboard.Api
{
    using GauntletLeaderboard.Api.Services;
    using Nancy;
    using Nancy.ModelBinding;
    using System.Collections.Generic;
    using GauntletLeaderboard.Api.Extensions;
    using Humanizer;
    using System;
    using System.Web;
    using System.Dynamic;
    using System.Linq;

    public class LeaderboardModule : NancyModule
    {
        public LeaderboardModule(ILeaderboardService leaderboardService)
            : base("/leaderboards")
        {
            Get["/"] = parameters =>
            {
                var result = leaderboardService.GetLeaderboardGroups();

                return PrepareResult(result);
            };

            Get["/{group}"] = parameters =>
            {
                string group = parameters.group;
                var result = leaderboardService.GetSubGroups(group);

                return PrepareResult(result);
            };

            Get["/{group}/{subgroup}"] = parameters =>
            {
                string group = parameters.group;
                string subGroup = parameters.subGroup;
                var result = leaderboardService.GetLeaderboardsBySubGroup(group, subGroup);

                return PrepareResult(result);
            };

            Get["/{id:int}"] = parameters =>
            {
                int id = parameters.id;
                var result = leaderboardService.GetLeaderboard(id);

                return PrepareResult(result);
            };

            Get["/{id:int}/entries"] = parameters =>
            {
                int id = parameters.id;
                var query = this.Bind<Query>();
                var result = leaderboardService.GetLeaderboardEntries(id, query.Page.HasValue ? query.Page.Value : 0, query.PageSize.HasValue ? query.PageSize.Value : 20);

                return PrepareResult(result);
            };
        }

        private dynamic PrepareResult<T>(T model)
        {
            var pagedResult = model as IPagedResult<object>;
            var enumerableResult = model as IEnumerable<object>;
            var typeName = model.GetType().GetInnerTypeName();
            var result = new Dictionary<string, object>();
            
            if (pagedResult != null)
            {
                var root = typeName.Pluralize();
                var url = new UriBuilder(this.Request.Url);
                var queryString = HttpUtility.ParseQueryString(this.Request.Url.Query);
                queryString.Set("pageSize", pagedResult.PageSize.ToString());
                dynamic links = new ExpandoObject();

                if (pagedResult.Next != null)
                {
                    queryString.Set("page", pagedResult.Next.Value.ToString());
                    url.Query = queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])).JoinWith("&");
                    links.Next = url.ToString();
                }

                if (pagedResult.Previous != null)
                {
                    queryString.Set("page", pagedResult.Previous.Value.ToString());
                    url.Query = queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])).JoinWith("&");
                    links.Previous = url.ToString();
                }

                queryString.Set("page", pagedResult.First.ToString());
                url.Query = queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])).JoinWith("&");
                links.First = url.ToString();

                queryString.Set("page", pagedResult.Last.ToString());
                url.Query = queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])).JoinWith("&");
                links.Last = url.ToString();

                result[root] = pagedResult.Page;
                result["links"] = links;
            }
            else if (enumerableResult != null)
            {
                var root = typeName.Pluralize();
                result[root] = enumerableResult;
            }
            else
            {
                result[typeName] = model;
            }

            return result;
        }
    }
}