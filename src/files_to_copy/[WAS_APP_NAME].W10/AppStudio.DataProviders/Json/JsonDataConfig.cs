// ***********************************************************************
// <copyright file="JsonDataConfig.cs" company="Alchiweb.fr">
//     Copyright MIT License / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using System.Net;

namespace AppStudio.DataProviders.Json
{
    public class JsonDataConfig
    {
        public JsonDataConfig()
        {
            NetCredential = null;
            SiteUrl = ApiPath = ApiFunction = "";
            UseXml = false;
            ElementsPath = null;
        }

        public string SiteUrl { get; set; }
        public string ApiPath { get; set; }
        public string ApiFunction { get; set; }
        public NetworkCredential NetCredential { get; set; }
        public bool UseXml { get; set; }
        public string ElementsPath { get; set; }
    }
}
