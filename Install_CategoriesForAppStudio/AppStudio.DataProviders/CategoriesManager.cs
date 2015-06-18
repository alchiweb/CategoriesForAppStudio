// ***********************************************************************
// <copyright file="IDataProviderWithCategories.cs" company="Alchiweb.fr">
//     Copyleft AGPL 3.0 / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using System.Collections.Generic;

namespace AppStudio.DataProviders
{
    public class CategoriesManager<TSchema> where TSchema : SchemaBase, ICategories, IHierarchical
    {
        private string _defaultCategoryId = null;
        private string _itemType = null;
        private string _categoryType = null;
        public string DefaultCategoryId { get { return _defaultCategoryId; } }
        public string ItemType { get { return _itemType; } }
        public string CategoryType { get { return _categoryType; } }
        public CategoriesManager(string defaultCategoryId = null, string categoryType = "category", string itemType = null)
        {
            _categoryType = categoryType;
            _itemType = itemType;
            _defaultCategoryId = defaultCategoryId;
        }

        public bool IsCategory(TSchema itemSchema)
        {
            return itemSchema != null &&
                !string.IsNullOrEmpty(CategoryType) &&
                ((string.IsNullOrEmpty(itemSchema.Type) && IsCategory(itemSchema._id)) || CategoryType == itemSchema.Type);
        }
        public bool IsFirstLevelCategory(TSchema itemSchema)
        {
            return IsCategory(itemSchema) &&
                itemSchema.ParentId == DefaultCategoryId;
        }

        public bool IsItem(TSchema itemSchema)
        {
            return !IsCategory(itemSchema) &&
                (string.IsNullOrEmpty(ItemType) || ((string.IsNullOrEmpty(itemSchema.Type) && IsItem(itemSchema._id)) || ItemType == itemSchema.Type));
        }
        public bool IsCategory(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && !string.IsNullOrEmpty(CategoryType) && itemId.Contains("_") && CategoryType == itemId.Substring(0, itemId.LastIndexOf("_"));
        }

        public bool IsItem(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && !IsCategory(itemId) && (string.IsNullOrEmpty(ItemType) || !itemId.Contains("_") || ItemType == itemId.Substring(0, itemId.LastIndexOf("_")));
        }
        public string AddTypeToId(string type, string id)
        {
            return string.IsNullOrWhiteSpace(id) ? null : string.Format("{0}_{1}", type, id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="catId"></param>
        /// <param name="itemsAndCategories"></param>
        /// <param name="itemsId">null if AllItemsForCategory not needed -> for optimisation</param>
        /// <returns></returns>
        public bool IsCategorieEmpty(string catId, IEnumerable<TSchema> itemsAndCategories, ref List<string> itemsId)
        {
            bool is_empty = true;

            foreach (var item in itemsAndCategories)
            {
                if (IsItem(item) && item.Categories != null && item.Categories.Contains(catId))
                {
                    if (itemsId == null)
                        return false;
                    else
                    {
                        if (is_empty)
                            is_empty = false;
                        itemsId.Add(item._id);
                    }
                }
            }
            foreach (var cat in itemsAndCategories)
            {
                if (IsCategory(cat) && cat.ParentId == catId)
                {
                    if (!IsCategorieEmpty(cat._id, itemsAndCategories, ref itemsId))
                    {
                        if (itemsId == null)
                            return false;
                        else
                        {
                            if (is_empty)
                                is_empty = false;
                        }
                    }
                }

            }
            return is_empty;
        }
    }
}
