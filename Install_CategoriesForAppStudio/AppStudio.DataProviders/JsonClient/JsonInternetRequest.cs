// ***********************************************************************
// <copyright file="JsonInternetRequest.cs" company="Alchiweb.fr">
//     Copyleft AGPL 3.0 / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using AppStudio.DataProviders.InternetClient;
using Newtonsoft.Json;
using System;
using Windows.Web.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppStudio.DataProviders.JsonClient
{
    internal static class JsonInternetRequest
    {
        internal static async Task<InternetRequestResult> DownloadAsync(JsonDataConfig config)
        {
            InternetRequestSettings settings = CreateSettings(config);
            InternetRequestResult result = new InternetRequestResult();

            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

                if (config.NetCredential != null)
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(config.NetCredential.UserName + ":" + config.NetCredential.Password)));

                if (!string.IsNullOrEmpty(settings.UserAgent))
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(settings.UserAgent);
                }

                HttpResponseMessage response = await httpClient.GetAsync(settings.RequestedUri);
                result.StatusCode = response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    FixInvalidCharset(response);
                    result.Result = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(result.Result) && config.UseXml)
                    {
                        XDocument xdoc = XDocument.Parse(result.Result);
                        result.Result = JsonConvert.SerializeXNode(xdoc);
                    }
                }
            }
            catch (Exception)
            {
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Result = string.Empty;
            }

            return result;
        }

        private static InternetRequestSettings CreateSettings(JsonDataConfig config)
        {
            string url = string.Format("{0}/{1}/{2}", config.SiteUrl, config.ApiPath, config.ApiFunction);
            InternetRequestSettings settings = new InternetRequestSettings
            {
                RequestedUri = new Uri(url),
                UserAgent = "NativeHost",
            };

            return settings;
        }

        private static void FixInvalidCharset(HttpResponseMessage response)
        {
            try
            {
                string charset = response.Content.Headers.ContentType.CharSet;
                if (charset.Contains("\""))
                {
                    response.Content.Headers.ContentType.CharSet = charset.Replace("\"", string.Empty);
                }
            }
            catch
            {
            }
        }
    }
}
