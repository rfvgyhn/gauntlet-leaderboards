using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Humanizer;
using System.Dynamic;
using Nancy;

namespace GauntletLeaderboard.Api.Extensions
{
    public static class NancyModuleExtensions
    {
        public static dynamic PrepareResult<T>(this INancyModule module, T model, Query query = null)
        {
            var pagedResult = model as IPagedResult<object>;
            var enumerableResult = model as IEnumerable<object>;
            var typeName = model.GetType().GetInnerTypeName();
            var result = new Dictionary<string, object>();

            if (pagedResult != null)
            {
                var root = typeName.Pluralize();
                var url = new UriBuilder(module.Request.Url);
                var queryString = HttpUtility.ParseQueryString(module.Request.Url.Query);
                dynamic links = new ExpandoObject();
                dynamic meta = new ExpandoObject();

                if (query.PageSize.HasValue)
                    queryString.Set("pageSize", pagedResult.PageSize.ToString());

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

                queryString.Remove("page");
                url.Query = queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])).JoinWith("&");
                links.First = url.ToString();

                queryString.Set("page", pagedResult.Last.ToString());
                url.Query = queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])).JoinWith("&");
                links.Last = url.ToString();

                meta.total = pagedResult.TotalItemCount;

                result[root] = pagedResult.Page;
                result["links"] = links;
                result["meta"] = meta;
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