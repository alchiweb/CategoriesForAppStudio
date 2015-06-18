// ***********************************************************************
// <copyright file="JsonParser.cs" company="Alchiweb.fr">
//     Copyleft AGPL 3.0 / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace AppStudio.DataProviders.JsonClient.Parser
{
    public class JsonParser<TSchema> : IParser<TSchema> where TSchema : JsonSchema, new()
    {
        public IEnumerable<TSchema> Parse(string data)
        {
            return Parse(data, null);
        }
        public IEnumerable<TSchema> Parse(string data, string elementsPath)
        {
            Collection<TSchema> resultToReturn = new Collection<TSchema>();
            var searchList = JsonConvert.DeserializeObject<JToken>(data);
            if (!string.IsNullOrWhiteSpace(elementsPath))
                searchList = searchList.SelectToken(elementsPath);
            if (searchList != null)
            {
                if (searchList.Type != JTokenType.Array)
                    searchList = new JArray(searchList);
                foreach (var item in searchList)
                {
                    TSchema item_schema = CreateSchema(item);
                    if (item_schema != null)
                        resultToReturn.Add(item_schema);
                }

            }
            return resultToReturn;
        }

        protected virtual TSchema CreateSchema(JToken token)
        {
            return new TSchema() { _id = (string)token.SelectToken("id") };
        }
    }
}
