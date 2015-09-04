// ***********************************************************************
// <copyright file="WordpressSchema.cs" company="Alchiweb.fr">
//     Copyright MIT License / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using AppStudio.DataProviders.Json;

namespace AppStudio.DataProviders.Wordpress
{
    public class WordpressSchema : JsonSchemaWithCategories
    {
        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public string ImageUrl { get; set; }

        public string Link { get; set; }

        public string Author { get; set; }

        public string Date { get; set; }


    }
}
