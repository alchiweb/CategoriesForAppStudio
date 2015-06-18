using System;

using System.Linq;
using System.Net;
using AppStudio.DataProviders.Core;
using Newtonsoft.Json.Linq;
using AppStudio.DataProviders.Wordpress;
using AppStudio.DataProviders.Wordpress.Parser;

namespace [WAS_APP_NAME].Sections
{
    public class [COLLECTION_CONFIG_NAME]Parser<TSchema> : WordpressParser<TSchema> where TSchema : WordpressSchema, new()
    {
        public [COLLECTION_CONFIG_NAME]Parser(string defaultCategoryId = null, string categoryType = "category", string itemType = "post") : base(defaultCategoryId, categoryType, itemType)
        {
        }

        protected override TSchema CreateSchema(JToken token)
        {
            TSchema item_schema = base.CreateSchema(token);
     
            if (IsCategory(item_schema))
                item_schema.ImageUrl = (string)token.SelectToken("meta.img_custom_field");
            return item_schema;
        }
    }
}
