// ***********************************************************************
// <copyright file="JsonHttpRequestResult.cs" company="Alchiweb.fr">
//     Copyright MIT License / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using Windows.Web.Http;

namespace AppStudio.DataProviders.Json
{
    internal class JsonHttpRequestResult
    {
        public JsonHttpRequestResult()
        {
            this.StatusCode = HttpStatusCode.Ok;
            this.Result = string.Empty;
        }

        public HttpStatusCode StatusCode { get; set; }

        public string Result { get; set; }

        public bool Success { get { return (this.StatusCode == HttpStatusCode.Ok && !string.IsNullOrEmpty(this.Result)); } }
    }
}
