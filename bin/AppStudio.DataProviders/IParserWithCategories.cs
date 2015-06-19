// ***********************************************************************
// <copyright file="IParserWithCategories.cs" company="Alchiweb.fr">
//     Copyright MIT License / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

namespace AppStudio.DataProviders
{
    public interface IParserWithCategories<T> : IParser<T> where T : SchemaBase, ICategories, IHierarchical
    {
        CategoriesManager<T> CategoriesManager { get; set; }
    }
}
