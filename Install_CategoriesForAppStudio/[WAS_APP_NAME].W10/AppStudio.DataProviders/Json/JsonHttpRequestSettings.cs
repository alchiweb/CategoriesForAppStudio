// ***********************************************************************
// <copyright file="JsonHttpRequestSettings.cs" company="Alchiweb.fr">
//     Copyright MIT License / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using System;
using System.Net;

namespace AppStudio.DataProviders.Json
{
    internal class JsonHttpRequestSettings
    {
        public JsonHttpRequestSettings()
        {
            this.Headers = new WebHeaderCollection();
        }

        public Uri RequestedUri { get; set; }

        public string UserAgent { get; set; }

        public WebHeaderCollection Headers { get; private set; }
    }
}
