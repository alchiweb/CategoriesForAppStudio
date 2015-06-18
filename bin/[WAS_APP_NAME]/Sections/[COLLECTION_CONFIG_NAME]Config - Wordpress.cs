using System;
using System.Collections.Generic;
using AppStudio.Common;
using AppStudio.Common.Actions;
using AppStudio.Common.Commands;
using AppStudio.Common.Navigation;
using AppStudio.DataProviders;
using AppStudio.DataProviders.Core;
using AppStudio.DataProviders.Wordpress;
using Windows.Storage;
using [WAS_APP_NAME].Config;
using [WAS_APP_NAME].ViewModels;
using AppStudio.DataProviders.Wordpress.Parser;
using AppStudio.DataProviders.JsonClient;

namespace [WAS_APP_NAME].Sections
{
    public class [COLLECTION_CONFIG_NAME]Config : SectionConfigBase<[COLLECTION_SCHEMA_NAME]Schema>
    {
        public override DataProviderBase<[COLLECTION_SCHEMA_NAME]Schema> DataProvider
        {
            get
            {
                return new JsonDataProviderWithCategories<[COLLECTION_SCHEMA_NAME]Schema>(new JsonDataConfig
                {
                    SiteUrl = "http://WORDPRESS_SITE_NAME.com",
                    ApiPath = "wp-json",
                    ApiFunction = "posts"
                },
                new JsonDataConfig
                {
                    SiteUrl = "http://WORDPRESS_SITE_NAME.com",
                    ApiPath = "wp-json",
                    ApiFunction = "taxonomies/category/terms"
                },
                new [COLLECTION_CONFIG_NAME]Parser<[COLLECTION_SCHEMA_NAME]Schema>()
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

    }
}
