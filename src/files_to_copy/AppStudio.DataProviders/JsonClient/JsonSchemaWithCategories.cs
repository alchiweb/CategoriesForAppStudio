// ***********************************************************************
// <copyright file="JsonSchemaWithCategories.cs" company="Alchiweb.fr">
//     Copyright MIT License / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using System.Collections.Generic;

namespace AppStudio.DataProviders.JsonClient
{
    public class JsonSchemaWithCategories : JsonSchema, IHierarchical, ICategories
    {
        public string ParentId { get; set; }
        public List<string> Categories { get; set; }
        public string Type { get; set; }
    }
}
