// ***********************************************************************
// <copyright file="PrestashopParser.cs" company="Alchiweb.fr">
//     Copyright MIT License / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using AppStudio.DataProviders.Json.Parser;

namespace AppStudio.DataProviders.Prestashop.Parser
{
    public class PrestashopParser<TSchema> : JsonParserWithCategories<TSchema> where TSchema : PrestashopSchema, new()
    {
        public PrestashopParser(string defaultCategoryId = null, string categoryType = "category", string itemType = "product") : base(defaultCategoryId, categoryType, itemType)
        {
        }

        protected override TSchema CreateSchema(JToken token)
        {
            string type = (string)token.Parent.Path;
            type = type.Substring(type.LastIndexOf(".") + 1);

            string site_url = (string)token.SelectToken("id_default_image.@xlink:href");
            if (site_url != null)
            {
                Uri site_uri = new Uri(site_url);
                site_url = String.Format("{0}://{1}", site_uri.Scheme, site_uri.Host);

            }
            string img_url = null;
            string img_id = (string)token.SelectToken("id_default_image.#cdata-section");
            if (!String.IsNullOrWhiteSpace(img_id))
            {
                img_url = img_id;
                for (int i = 0; i < img_id.Length; i++)
                    img_url = img_url.Insert(2 * i, "/");
                img_url = String.Format("{0}/img/p{1}/{2}.jpg", site_url, img_url, img_id);
            }
            var item_schema = new TSchema();
            item_schema.Type = type;
            item_schema._id = CategoriesManager.AddTypeToId(type, (string)token.SelectToken("id.#cdata-section"));

            if (CategoriesManager.IsCategory(item_schema))
            {
                string is_root_category = (string)token.SelectToken("is_root_category.#cdata-section");
                if (!string.IsNullOrEmpty(is_root_category) && is_root_category == "1")
                    item_schema.ParentId = null;
                else
                {
                    string parent_id = (string)token.SelectToken("id_parent.#cdata-section");
                    if (!string.IsNullOrEmpty(parent_id) && parent_id == "0")
                        return null;
                    item_schema.ParentId = CategoriesManager.AddTypeToId(type, parent_id);
                }
            }
            else
            {
            }
            item_schema.Categories = GetAssociationsId(token);
            item_schema.Date = (string)token.SelectToken("name.language.#cdata-section");
            item_schema.Link = (string)token.SelectToken("id_default_image.@xlink:href");
            item_schema.ImageUrl = img_url;
            item_schema.Title = (string)token.SelectToken("name.language.#cdata-section");
            item_schema.Content = (string)token.SelectToken("name.language.#cdata-section");
            item_schema.Summary = (string)token.SelectToken("name.language.#cdata-section");
            return item_schema;
        }

        protected List<string> GetAssociationsId(JToken token, string associationsListName = "associations")
        {
            List<string> associations = new List<string>();
            var token_associations = token.SelectToken(associationsListName);
            if (token_associations != null && token_associations.HasValues)
            {
                foreach (var association_type in token_associations)
                {
                    string categories_type_name = ((JProperty)association_type).Name;
                    string category_type_name = (string)token_associations.SelectToken(String.Format("{0}.@nodeType", categories_type_name));
                    var object_or_array = token_associations.SelectToken(String.Format("{0}.{1}", categories_type_name, category_type_name));
                    if (object_or_array != null && object_or_array.HasValues)
                    {
                        if (object_or_array.Type != JTokenType.Array)
                            object_or_array = new JArray(object_or_array);
                        foreach (var association in object_or_array)
                        {
                            associations.Add(CategoriesManager.AddTypeToId(category_type_name, (string)association.SelectToken("id.#cdata-section")));
                        }
                    }

                }
            }

            return associations;
        }
    }
}
