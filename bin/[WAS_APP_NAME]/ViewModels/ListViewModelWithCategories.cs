// ***********************************************************************
// <copyright file="ListViewModelWithCategories.cs" company="Alchiweb.fr">
//     Copyright MIT License / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AppStudio.Common.Cache;
using AppStudio.Common.DataSync;
using AppStudio.DataProviders;
using [WAS_APP_NAME].Config;

namespace [WAS_APP_NAME].ViewModels
{
    public class ListViewModelWithCategories<T> : ListViewModel<T> where T : SchemaBase, ICategories, IHierarchical
    {
        private int _visibleItems;
        private SectionConfigBase<T> _sectionConfig;
        private bool _hasMoreItems;
        private ObservableCollection<ItemViewModel> _currentLevelCategories;
        private ObservableCollection<ItemViewModel> _allCategories;
        private ObservableCollection<ItemViewModel> _categoriesNotEmpty;
        private ObservableCollection<ItemViewModel> _allItems;
        private ObservableCollection<ItemViewModel> _allItemsForCategory;
        private ItemViewModel _selectedCategory;
        public ItemViewModel SelectedCategory
        {
            get { return _selectedCategory; }
            set { SetProperty(ref _selectedCategory, value); }
        }
        public ObservableCollection<ItemViewModel> Categories
        {
            get
            {
                switch ((_sectionConfig.DataProvider as IDataProviderWithCategories<T>).VisibleCategories)
                {
                    case VisibleCategoriesType.All:
                        return _allCategories;
                    case VisibleCategoriesType.NotEmpty:
                        return _categoriesNotEmpty;
                    case VisibleCategoriesType.CurrentLevel:
                    default:
                        return _currentLevelCategories;
                }
            }
        }
        public new ObservableCollection<ItemViewModel> Items
        {
            get
            {
                switch ((_sectionConfig.DataProvider as IDataProviderWithCategories<T>).VisibleItems)
                {
                    case VisibleItemsType.AllForCurrentCategory:
                        return _allItemsForCategory;
                    case VisibleItemsType.All:
                        return _allItems;
                    case VisibleItemsType.CurrentLevel:
                    default:
                        return base.Items;
                }
            }
        }
        public ObservableCollection<ItemViewModel> CurrentLevelItems
        {
            get { return base.Items; }
        }

        public ObservableCollection<ItemViewModel> CurrentLevelCategories
        {
            get { return _currentLevelCategories; }
            private set { SetProperty(ref _currentLevelCategories, value); }
        }
        public ObservableCollection<ItemViewModel> AllCategories
        {
            get { return _allCategories; }
            private set { SetProperty(ref _allCategories, value); }
        }
        public ObservableCollection<ItemViewModel> CategoriesNotEmpty
        {
            get { return _categoriesNotEmpty; }
            private set { SetProperty(ref _categoriesNotEmpty, value); }
        }
        public ObservableCollection<ItemViewModel> AllItems
        {
            get { return _allItems; }
            private set { SetProperty(ref _allItems, value); }
        }
        public ObservableCollection<ItemViewModel> AllItemsForCategory
        {
            get { return _allItemsForCategory; }
            private set { SetProperty(ref _allItemsForCategory, value); }
        }
        public ListViewModelWithCategories(SectionConfigBase<T> sectionConfig, int visibleItems = 0)
            : base(sectionConfig, visibleItems)
        {
            _sectionConfig = sectionConfig;
            _visibleItems = visibleItems;
            CurrentLevelCategories = new ObservableCollection<ItemViewModel>();
            AllCategories = new ObservableCollection<ItemViewModel>();
            CategoriesNotEmpty = new ObservableCollection<ItemViewModel>();
            AllItems = new ObservableCollection<ItemViewModel>();
            AllItemsForCategory = new ObservableCollection<ItemViewModel>();
        }

        protected override void ParseItems(CachedContent<T> content, ItemViewModel selectedCategory)
        {
            var categories_manager = (_sectionConfig.DataProvider as IDataProviderWithCategories<T>).Parser.CategoriesManager;

            var parsedItems = new List<ItemViewModel>();
            var allParsedItems = new List<ItemViewModel>();
            var parsedCategories = new List<ItemViewModel>();
            var allParsedCategories = new List<ItemViewModel>();
            string selected_category_id;
            string page_title;

            if (selectedCategory != null)
            {
                SelectedCategory = selectedCategory;
                selected_category_id = selectedCategory._id;
                page_title = selectedCategory.PageTitle;
            }
            else
            {
                selected_category_id = categories_manager.DefaultCategoryId;
                page_title = "";
            }

            foreach (var category in content.Items)
            {
                if (categories_manager.IsCategory(category))
                {
                    if (selectedCategory == null || selectedCategory._id != category._id)
                    {
                        var parsed_category = new ItemViewModel
                        {
                            _id = category._id,
                            NavigationInfo = _sectionConfig.ListPage.NavigationInfo(category),
                            ParentId = category.ParentId
                        };
                        _sectionConfig.ListPage.LayoutBindings(parsed_category, category);
                        parsed_category.PageTitle = string.Format(parsed_category.PageTitle, page_title, parsed_category.Title);

                        allParsedCategories.Add(parsed_category);
                        if (selected_category_id == parsed_category.ParentId)
                            parsedCategories.Add(parsed_category);

                        if (selected_category_id == category._id) // && selectedCategory == null 
                        {
                            SelectedCategory = parsed_category;     // default category
                        }
                    }
                }
            }
            if (SelectedCategory == null && !string.IsNullOrEmpty(selected_category_id))    // selected category not found
                selected_category_id = null;

            foreach (var item in content.Items)
            {
                if (categories_manager.IsItem(item))
                {
                    var parsedItem = new ItemViewModel
                    {
                        _id = item._id,
                        NavigationInfo = _sectionConfig.ListPage.NavigationInfo(item),
                    };

                    _sectionConfig.ListPage.LayoutBindings(parsedItem, item);
                    parsedItem.ParentId = selected_category_id;
                    parsedItem.PageTitle = string.Format(parsedItem.PageTitle, page_title);

                    allParsedItems.Add(parsedItem);
                    if ((_visibleItems <= 0 || parsedItems.Count < _visibleItems) && (item.Categories != null && item.Categories.Contains(selected_category_id)))
                    {
                        parsedItem.ParentId = selected_category_id;
                        parsedItems.Add(parsedItem);
                    }
                }
            }
            CurrentLevelCategories.Sync(parsedCategories);
            AllCategories.Sync(allParsedCategories);
            base.Items.Sync(parsedItems);
            AllItems.Sync(allParsedItems);
            var categories_id = new List<string>();
            allParsedCategories = new List<ItemViewModel>();
            List<string> items_for_category = new List<string>();       // null if AllItemsForCategory not needed -> more efficience
            foreach (var cat in parsedCategories)
            {
                if (!categories_manager.IsCategorieEmpty(cat._id, content.Items, ref items_for_category))
                    allParsedCategories.Add(cat);
            }
            CategoriesNotEmpty.Sync(allParsedCategories);

            if (items_for_category != null && items_for_category.Count > 0)
            {
                foreach (var item in allParsedItems)
                {
                    if ((_visibleItems <= 0 || parsedItems.Count < _visibleItems) && items_for_category.Contains(item._id))
                        parsedItems.Add(item);
                }
            }
            AllItemsForCategory.Sync(parsedItems);

            HasMoreItems = content.Items.Count() > Items.Count;
            if (selected_category_id != categories_manager.DefaultCategoryId)
                Actions.RemoveAll(action => action.Name == "RefreshButton");

        }

        public new bool HasMoreItems
        {
            get { return _hasMoreItems; }
            private set { SetProperty(ref _hasMoreItems, value); }
        }
    }
}
