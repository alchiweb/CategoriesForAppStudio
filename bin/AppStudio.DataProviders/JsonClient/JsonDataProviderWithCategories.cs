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
using AppStudio.DataProviders.JsonClient.Parser;
using System.Collections.ObjectModel;

namespace AppStudio.DataProviders.JsonClient
{
    public class JsonDataProviderWithCategories<T> : JsonDataProvider<T>, IDataProviderWithCategories<T> where T : JsonSchemaWithCategories, new()
    {
        public IParserWithCategories<T> Parser { get { return _parser as IParserWithCategories<T>; } }

        public VisibleItemsType VisibleItems { get; set; }
        public VisibleCategoriesType VisibleCategories { get; set; }

        protected JsonDataConfig _category_config;

        public JsonDataProviderWithCategories(JsonDataConfig config, JsonDataConfig categoryConfig = null, VisibleItemsType visibleItems = VisibleItemsType.CurrentLevel, VisibleCategoriesType visibleCategories = VisibleCategoriesType.CurrentLevel)
            : base(config, new JsonParserWithCategories<T>())
        {
            _category_config = categoryConfig;
        }
        public JsonDataProviderWithCategories(JsonDataConfig config, JsonDataConfig categoryConfig, JsonParserWithCategories<T> parser, VisibleItemsType visibleItems = VisibleItemsType.CurrentLevel, VisibleCategoriesType visibleCategories = VisibleCategoriesType.CurrentLevel)
            : base(config, parser)
        {
            VisibleItems = visibleItems;
            VisibleCategories = visibleCategories;
            _category_config = categoryConfig;
        }
        //public JsonDataProviderWithCategories(JsonDataConfig config, JsonParserWithCategories<T> parser)
        //    : this(config, null, parser)
        //{
        //}

        public override async Task<IEnumerable<T>> LoadDataAsync()
        {
            var result = await JsonInternetRequest.DownloadAsync(_config);
            if (result.Success)
            {
                Collection<T> items = (_parser as JsonParserWithCategories<T>).Parse(result.Result, _config.ElementsPath) as Collection<T>;
                if (_category_config != null)
                {
                    result = await JsonInternetRequest.DownloadAsync(_category_config);
                    if (result.Success)
                    {
                        Collection<T> items_to_add = (_parser as JsonParserWithCategories<T>).Parse(result.Result, _category_config.ElementsPath) as Collection<T>;
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
    }
}
