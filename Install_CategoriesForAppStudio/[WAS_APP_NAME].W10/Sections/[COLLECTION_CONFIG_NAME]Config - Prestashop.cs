using System;
using System.Collections.Generic;
using AppStudio.Uwp;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.Navigation;
using AppStudio.DataProviders;
using AppStudio.DataProviders.Core;
using AppStudio.DataProviders.Wordpress;
using Windows.Storage;
using [WAS_APP_NAME].Config;
using [WAS_APP_NAME].ViewModels;
using AppStudio.DataProviders.Prestashop.Parser;
using AppStudio.DataProviders.Json;
using System.Net;

namespace [WAS_APP_NAME].Sections
{
    public class [COLLECTION_CONFIG_NAME]Config : SectionConfigBase<JsonDataConfig, [COLLECTION_SCHEMA_NAME]Schema>
    {
        public override JsonDataConfig Config
        {
            get
            {
                return new JsonDataConfig
                {
                    SiteUrl = "http://PRESTASHOP_SITE_NAME.com",
                    ApiPath = "api",
                    ApiFunction = "products?display=full",
                    UseXml = true,
                    ElementsPath = "prestashop.products.product",
                    NetCredential = new NetworkCredential("API_KEY", "", "")
                };
            }
        }
        public override DataProviderBase<JsonDataConfig, [COLLECTION_SCHEMA_NAME]Schema> DataProvider
        {
            get
            {
                return new JsonDataProviderWithCategories<[COLLECTION_SCHEMA_NAME]Schema>(null,
                new JsonDataConfig
                {
                    SiteUrl = "http://PRESTASHOP_SITE_NAME.com",
                    ApiPath = "api",
                    ApiFunction = "categories?display=full",
                    UseXml = true,
                    ElementsPath = "prestashop.categories.category",
                    NetCredential = new NetworkCredential("API_KEY", "", "")
                },
                new PrestashopParser<[COLLECTION_SCHEMA_NAME]Schema>("category_2"),
                VisibleItemsType.AllForCurrentCategory,
                VisibleCategoriesType.NotEmpty

                );
            }
        }

        public override NavigationInfo ListNavigationInfo
        {
            get
            {
                return NavigationInfo.FromPage("[COLLECTION_CONFIG_NAME]ListPage");
            }
        }

        public override ListPageConfig<[COLLECTION_SCHEMA_NAME]Schema> ListPage
        {
            get
            {
                var categories_manager = ((IDataProviderWithCategories<[COLLECTION_SCHEMA_NAME]Schema>)DataProvider).Parser.CategoriesManager;
                var title = "[COLLECTION_NAME]";
                return new ListPageConfig<[COLLECTION_SCHEMA_NAME]Schema>
                {
                    Title = title,

                    LayoutBindings = (viewModel, item) =>
                    {
                        viewModel.PageTitle = categories_manager.IsCategory(item) ?
                            (categories_manager.DefaultCategoryId == item.ParentId ? title + " : {1}" : "{0} / {1}")
                            : "{0}";
                        viewModel.Title = item.Title;
                        viewModel.SubTitle = item.Summary;
                        viewModel.Description = item.Content;
                        viewModel.Image = item.ImageUrl;
                        viewModel.ParentId = item.ParentId;
                    },
                    NavigationInfo = (item) =>
                    {
                        if (categories_manager.IsCategory(item))
                            return NavigationInfo.FromPage("[COLLECTION_CONFIG_NAME]ListPage", true);
                        return NavigationInfo.FromPage("[COLLECTION_CONFIG_NAME]DetailPage", true);
                    }
                };
            }
        }

        public override DetailPageConfig<[COLLECTION_SCHEMA_NAME]Schema> DetailPage
        {
            get
            {
                var bindings = new List<Action<ItemViewModel, [COLLECTION_SCHEMA_NAME]Schema>>();

                bindings.Add((viewModel, item) =>
                {
                    viewModel.PageTitle = item.Title;
                    viewModel.Title = item.Title;
                    viewModel.Description = item.Summary;
                    viewModel.Image = item.ImageUrl;
                    viewModel.Content = item.Content;
                    viewModel.ParentId = item.ParentId;
                });


                var actions = new List<ActionConfig<[COLLECTION_SCHEMA_NAME]Schema>>
                {
                };

                return new DetailPageConfig<[COLLECTION_SCHEMA_NAME]Schema>
                {
                    Title = "[COLLECTION_NAME]",
                    LayoutBindings = bindings,
                    Actions = actions
                };
            }
        }
        public override string PageTitle
        {
            get { return "[COLLECTION_NAME]"; }
        }
    }
}
