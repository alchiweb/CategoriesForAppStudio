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

namespace AppStudio.DataProviders
{
    public enum VisibleItemsType
    {
        CurrentLevel,
        All,
        AllForCurrentCategory
    }
    public enum VisibleCategoriesType
    {
        CurrentLevel,
        All,
        NotEmpty
    }

    public interface IDataProviderWithCategories<T> where T : SchemaBase, ICategories, IHierarchical
    {
        IParserWithCategories<T> Parser { get; }
        VisibleItemsType VisibleItems { get; set; }
        VisibleCategoriesType VisibleCategories { get; set; }
    }
}
