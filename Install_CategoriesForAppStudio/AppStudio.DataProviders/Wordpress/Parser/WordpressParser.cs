// ***********************************************************************
// <copyright file="WordpressSchema.cs" company="Alchiweb.fr">
//     Copyleft AGPL 3.0 / 2015 Alchiweb.fr
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
using AppStudio.DataProviders.JsonClient.Parser;

namespace AppStudio.DataProviders.Wordpress.Parser
{
    public class WordpressParser<TSchema> : JsonParserWithCategories<TSchema> where TSchema : WordpressSchema, new()
    {
        public WordpressParser(string defaultCategoryId = null, string categoryType = "category", string itemType = "post") : base(defaultCategoryId, categoryType, itemType)
        {
        }

        protected override TSchema CreateSchema(JToken token)
        {
            var item_schema = new TSchema();
            string item_type = (string)token.SelectToken("type");
            // Properties for categories
            if (String.IsNullOrEmpty(item_type))
            {
                item_type = (string)token.SelectToken("taxonomy");
                item_schema.Title = (string)token.SelectToken("name");
                item_schema.Summary = (string)token.SelectToken("description");
                item_schema.Content = (string)token.SelectToken("description");
                item_schema.Date = DateTime.Now.ToString();
            }
            // Properties for items
            else
            {
                item_schema.Date = (string)token.SelectToken("modified");
                item_schema.ImageUrl = (string)token.SelectToken("featured_image.guid");
                item_schema.Title = (string)token.SelectToken("title");
                item_schema.Summary = (string)token.SelectToken("content");
                item_schema.Content = (string)token.SelectToken("content");
            }
            // Common properties
            item_schema.Type = item_type;
            item_schema._id = CategoriesManager.AddTypeToId(item_type, (string)token.SelectToken("ID"));
            item_schema.ParentId = CategoriesManager.AddTypeToId(item_type, (string)token.SelectToken("parent.ID"));
            item_schema.Categories = GetTermsId(token);
            item_schema.Link = (string)token.SelectToken("link");
            return item_schema;
        }

        protected List<string> GetTermsId(JToken token, string termsListName = "terms")
        {
            List<string> terms = new List<string>();
            var token_terms = token.SelectToken(termsListName);
            if (token_terms != null && token_terms.HasValues)
            {
                foreach (var term_type in token_terms)
                {
                    string term_type_name = ((JProperty)term_type).Name;
                    foreach (var term in token_terms.SelectToken(term_type_name))
                    {
                        terms.Add(CategoriesManager.AddTypeToId(term_type_name, (string)term.SelectToken("ID")));
                    }
                }
            }

            return terms;
        }
    }
}
