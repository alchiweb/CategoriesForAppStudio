// ***********************************************************************
// <copyright file="DetailViewModelWithCategories.cs" company="Alchiweb.fr">
//     Copyleft AGPL 3.0 / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using System.Collections.ObjectModel;
using System.Linq;
using AppStudio.Common.Actions;
using AppStudio.Common.Cache;
using AppStudio.DataProviders;
using [WAS_APP_NAME].Config;
using System.Collections.Generic;

namespace [WAS_APP_NAME].ViewModels
{
    public class DetailViewModelWithCategories<T> : DetailViewModel<T> where T : SchemaBase, ICategories, IHierarchical
    {
        private SectionConfigBase<T> _sectionConfig;

        private ItemViewModel _selectedCategory;
        private ObservableCollection<ComposedItemViewModel> _allItems;
        private ObservableCollection<ComposedItemViewModel> _allItemsForCategory;
        public ItemViewModel SelectedCategory
        {
            get { return _selectedCategory; }
            set { SetProperty(ref _selectedCategory, value); }
        }
        public new ObservableCollection<ComposedItemViewModel> Items
        {
            get
            {
                switch ((_sectionConfig.DataProvider as IDataProviderWithCategories<T>).VisibleItems)
                {
                    case VisibleItemsType.All:
                        return _allItems;
                    case VisibleItemsType.AllForCurrentCategory:
                        return _allItemsForCategory;
                    case VisibleItemsType.CurrentLevel:
                    default:
                        return base.Items;
                }
            }
        }
        public ObservableCollection<ComposedItemViewModel> CurrentLevelItems
        {
            get { return base.Items; }
        }

        public ObservableCollection<ComposedItemViewModel> AllItems
        {
            get { return _allItems; }
            private set { SetProperty(ref _allItems, value); }
        }
        public ObservableCollection<ComposedItemViewModel> AllItemsForCategory
        {
            get { return _allItemsForCategory; }
            private set { SetProperty(ref _allItemsForCategory, value); }
        }
        public DetailViewModelWithCategories(SectionConfigBase<T> sectionConfig)
            : base(sectionConfig)
        {
            _sectionConfig = sectionConfig;
            AllItems = new ObservableCollection<ComposedItemViewModel>();
            AllItemsForCategory = new ObservableCollection<ComposedItemViewModel>();
        }


        protected override void ParseItems(CachedContent<T> content, ItemViewModel selectedItem)
        {
            var categories_manager = (_sectionConfig.DataProvider as IDataProviderWithCategories<T>).Parser.CategoriesManager;
            string selected_category_id = null;
            if (selectedItem != null)
                selected_category_id = selectedItem.ParentId;

            var allParsedCategories = new List<ItemViewModel>();
            
            foreach (var category in content.Items)
            {
                if (categories_manager.IsCategory(category))
                {

                    ItemViewModel parsed_category = new ItemViewModel
                    {
                        _id = category._id,
                        NavigationInfo = _sectionConfig.ListPage.NavigationInfo(category),
                        ParentId = category.ParentId
                    };
                    _sectionConfig.ListPage.LayoutBindings(parsed_category, category);
                    allParsedCategories.Add(parsed_category);
                    if (category._id == selected_category_id)
                    {
                        SelectedCategory = parsed_category;
                    }
                }
            }

            foreach (var item in content.Items)
            {
                if (categories_manager.IsItem(item))
                {
                    var composedItem = new ComposedItemViewModel
                    {
                        _id = item._id
                    };

                    foreach (var binding in _sectionConfig.DetailPage.LayoutBindings)
                    {
                        var parsedItem = new ItemViewModel
                        {
                            _id = item._id,
                        };
                        binding(parsedItem, item);
                        composedItem.Add(parsedItem);
                    }

                    composedItem.Actions = _sectionConfig.DetailPage.Actions
                                                                    .Select(a => new ActionInfo
                                                                    {
                                                                        Command = a.Command,
                                                                        CommandParameter = a.CommandParameter(item),
                                                                        Style = a.Style,
                                                                        Text = a.Text,
                                                                        Type = ActionType.Primary
                                                                    })
                                                                    .ToList();
                    AllItems.Add(composedItem);
                    if (item.Categories != null && item.Categories.Contains(selected_category_id))
                    {
                        base.Items.Add(composedItem);
                    }
                }
            }

            var categories_id = new List<string>();
            List<string> items_for_category = new List<string>();       // null if AllItemsForCategory not needed -> more efficience
            categories_manager.IsCategorieEmpty(selected_category_id, content.Items, ref items_for_category);

            if (items_for_category != null && items_for_category.Count > 0)
            {
                foreach (var item in AllItems)
                {
                    if (items_for_category.Contains(item._id))
                        AllItemsForCategory.Add(item);
                }
            }

            if (selectedItem != null)
            {
                SelectedItem = AllItems.FirstOrDefault(i => i._id == selectedItem._id);
            }
        }
    }
}
