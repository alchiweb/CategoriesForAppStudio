// ***********************************************************************
// <copyright file="JsonDataProvider.cs" company="Alchiweb.fr">
//     Copyright MIT License / 2015 Alchiweb.fr
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
using AppStudio.DataProviders.Json.Parser;
using System.Collections.ObjectModel;

namespace AppStudio.DataProviders.Json
{
    public class JsonDataProvider<T> : DataProviderBase<JsonDataConfig, T> where T : JsonSchema, new()
    {
        public override async Task<IEnumerable<T>> LoadDataAsync(JsonDataConfig config)
        {
            return await LoadDataAsync(config, new JsonParser<T>());
        }
        public override async Task<IEnumerable<T>> LoadDataAsync(JsonDataConfig config, IParser<T> parser)
        {
            Assertions(config, parser);

            var result = await JsonHttpRequest.DownloadAsync(config);

            if (result.Success)
            {
                return (parser as JsonParser<T>).Parse(result.Result, config.ElementsPath) as Collection<T>;
            }
            throw new RequestFailedException(result.StatusCode, result.Result);
        }

        private static void Assertions(JsonDataConfig config, IParser<T> parser)
        {
            if (config == null)
            {
                throw new ConfigNullException();
            }
            if (parser == null)
            {
                throw new ParserNullException();
            }
            if (config.ElementsPath == null)
            {
                throw new ConfigParameterNullException("ElementsPath");
            }
        }
    }
}
