// ***********************************************************************
// <copyright file="IHierarchical.cs" company="Alchiweb.fr">
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
    public interface IHierarchical
    {
        string ParentId { get; set; }
    }

    //public interface IHierarchical<T>
    //{
    //    T Parent { get; set; }
    //    ObservableCollection<T> Childs { get; set; }
    //}
}
