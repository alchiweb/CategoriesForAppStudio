// ***********************************************************************
// <copyright file="JsonParserWithCategories.cs" company="Alchiweb.fr">
//     Copyleft AGPL 3.0 / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using Newtonsoft.Json.Linq;

namespace AppStudio.DataProviders.JsonClient.Parser
{
    public class JsonParserWithCategories<TSchema> : JsonParser<TSchema>, IParserWithCategories<TSchema> where TSchema : JsonSchemaWithCategories, new()
    {
        public CategoriesManager<TSchema> CategoriesManager { get; set; }
        public JsonParserWithCategories(string defaultCategoryId = null, string categoryType = "category", string itemType = null)
        {
            CategoriesManager = new CategoriesManager<TSchema>(defaultCategoryId, categoryType, itemType);
        }
        protected override TSchema CreateSchema(JToken token)
        {
            return new TSchema() { _id = (string)token["id"] };
        }

    }
}
