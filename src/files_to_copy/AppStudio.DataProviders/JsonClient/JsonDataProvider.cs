// ***********************************************************************
// <copyright file="JsonDataProvider.cs" company="Alchiweb.fr">
//     Copyleft AGPL 3.0 / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using AppStudio.DataProviders.Exceptions;
using AppStudio.DataProviders.JsonClient.Parser;
using System.Collections.ObjectModel;

namespace AppStudio.DataProviders.JsonClient
{
    public class JsonDataProvider<T> : DataProviderBase<JsonDataConfig, T> where T : JsonSchema, new()
    {
        public JsonDataProvider(JsonDataConfig config)
            : base(config, new JsonParser<T>())
        {
        }
        public JsonDataProvider(JsonDataConfig config, JsonParser<T> parser)
            : base(config, parser)
        {
        }

        public override async Task<IEnumerable<T>> LoadDataAsync()
        {
            var result = await JsonInternetRequest.DownloadAsync(_config);

            if (result.Success)
            {
                return (_parser as JsonParser<T>).Parse(result.Result, _config.ElementsPath) as Collection<T>;
            }
            throw new RequestFailedException();
        }


    }
}
