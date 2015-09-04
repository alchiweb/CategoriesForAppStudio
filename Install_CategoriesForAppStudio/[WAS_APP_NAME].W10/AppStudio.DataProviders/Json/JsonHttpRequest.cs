// ***********************************************************************
// <copyright file="JsonHttpRequest.cs" company="Alchiweb.fr">
//     Copyright MIT License / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using Newtonsoft.Json;
using System;
using Windows.Web.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Web.Http.Filters;

namespace AppStudio.DataProviders.Json
{
    internal static class JsonHttpRequest
    {
        internal static async Task<JsonHttpRequestResult> DownloadAsync(JsonDataConfig config)
        {
            JsonHttpRequestSettings settings = CreateSettings(config);

            var result = new JsonHttpRequestResult();

            var filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;

            var httpClient = new HttpClient(filter);

            if (config.NetCredential != null)
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(config.NetCredential.UserName + ":" + config.NetCredential.Password)));
            
            if (!string.IsNullOrEmpty(settings.UserAgent))
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(settings.UserAgent);
            }

            HttpResponseMessage response = await httpClient.GetAsync(settings.RequestedUri);
            result.StatusCode = response.StatusCode;
            FixInvalidCharset(response);
            result.Result = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(result.Result) && config.UseXml)
            {
                XDocument xdoc = XDocument.Parse(result.Result);
                result.Result = JsonConvert.SerializeXNode(xdoc);
            }

            return result;
        }

        private static JsonHttpRequestSettings CreateSettings(JsonDataConfig config)
        {
            string url = string.Format("{0}/{1}/{2}", config.SiteUrl, config.ApiPath, config.ApiFunction);
            JsonHttpRequestSettings settings = new JsonHttpRequestSettings
            {
                RequestedUri = new Uri(url),
                UserAgent = "NativeHost",
            };

            return settings;
        }


        private static void FixInvalidCharset(HttpResponseMessage response)
        {
            if (response != null && response.Content != null && response.Content.Headers != null
                && response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.CharSet != null)
            {
                // Fix invalid charset returned by some web sites.
                string charset = response.Content.Headers.ContentType.CharSet;
                if (charset.Contains("\""))
                {
                    response.Content.Headers.ContentType.CharSet = charset.Replace("\"", string.Empty);
                }
            }
        }

    }
}
