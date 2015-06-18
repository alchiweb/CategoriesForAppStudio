// ***********************************************************************
// <copyright file="ICategories.cs" company="Alchiweb.fr">
//     Copyleft AGPL 3.0 / 2015 Alchiweb.fr
// </copyright>
// <summary>
//     Added by CategoriesForAppStudio
//     Created by Hervé PHILIPPE
//     alchiweb@live.fr / alchiweb.fr
// </summary>
// ***********************************************************************

using System.Collections.Generic;

namespace AppStudio.DataProviders
{
    public interface ICategories
    {
        string Type { get; set; }
        List<string> Categories { get; }
    }

}
