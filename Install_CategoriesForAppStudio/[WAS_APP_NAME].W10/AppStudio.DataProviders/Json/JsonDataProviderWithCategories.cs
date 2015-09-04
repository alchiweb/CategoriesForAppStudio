// ***********************************************************************
// <copyright file="JsonDataProviderWithCategories.cs" company="Alchiweb.fr">
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
    public class JsonDataProviderWithCategories<T> : JsonDataProvider<T>, IDataProviderWithCategories<T> where T : JsonSchemaWithCategories, new()
    {
        private JsonParserWithCategories<T> _parser;
        public IParserWithCategories<T> Parser { get { return _parser; } }

        public VisibleItemsType VisibleItems { get; set; }
        public VisibleCategoriesType VisibleCategories { get; set; }

        protected JsonDataConfig _category_config;

        public JsonDataProviderWithCategories(JsonDataConfig config, JsonDataConfig categoryConfig = null, VisibleItemsType visibleItems = VisibleItemsType.CurrentLevel, VisibleCategoriesType visibleCategories = VisibleCategoriesType.CurrentLevel)
        {
            _parser = new JsonParserWithCategories<T>();
            _category_config = categoryConfig;
        }
        public JsonDataProviderWithCategories(JsonDataConfig config, JsonDataConfig categoryConfig, JsonParserWithCategories<T> parser, VisibleItemsType visibleItems = VisibleItemsType.CurrentLevel, VisibleCategoriesType visibleCategories = VisibleCategoriesType.CurrentLevel)
        {
            _parser = parser;
            VisibleItems = visibleItems;
            VisibleCategories = visibleCategories;
            _category_config = categoryConfig;
        }
        public override async Task<IEnumerable<T>> LoadDataAsync(JsonDataConfig config)
        {
            return await LoadDataAsync(config, Parser);
        }
        public override async Task<IEnumerable<T>> LoadDataAsync(JsonDataConfig config, IParser<T> parser)
        {
            Assertions(config, parser);

            var result = await JsonHttpRequest.DownloadAsync(config);
            if (result.Success)
            {
                Collection<T> items = (parser as JsonParserWithCategories<T>).Parse(result.Result, config.ElementsPath) as Collection<T>;
                if (_category_config != null)
                {
                    result = await JsonHttpRequest.DownloadAsync(_category_config);
                    if (result.Success)
                    {
                        Collection<T> items_to_add = (parser as JsonParserWithCategories<T>).Parse(result.Result, _category_config.ElementsPath) as Collection<T>;
                        foreach (T item in items_to_add)
                            items.Add(item);
                    }
                    else
                        throw new RequestFailedException();

                }
                return items;
            }
            throw new RequestFailedException();
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
