﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;

namespace CmsForWas
{
    class Program
    {
        static void Main(string[] args)
        {
            string app_name;
            string dest_path;
            bool with_base;
            List<string> collections;

            if (ParseArgs(args, out app_name, out collections, out dest_path, out with_base))
            {
                Console.WriteLine(InstallFiles(dest_path, app_name, collections, with_base));
            }
            else
                Console.WriteLine(string.Format("Syntax:\nCategoriesForAppStudio {{WAS_APP_NAME}} -bc|-c {{NAME_COL_CONFIG_1}} {{NAME_COL_CONFIG_2}} ... {{NAME_COL_CONFIG_N}} [-path {{PATH_DEST}}]"));
            //Console.Read();
        }
        static bool ParseArgs(string[] args, out string appName, out List<string> collections, out string destPath, out bool withBase)
        {
            const string PARAMETER_BC = "-bc";
            const string PARAMETER_C = "-c";

            collections = new List<string>();
            destPath = appName = "";
            withBase = false;

            if (args.Length < 2)
                return false;
            appName = args[0];
            if (string.IsNullOrWhiteSpace(appName))
                return false;
            switch (args[1])
            {
                case PARAMETER_BC:
                    withBase = true;
                    break;
                case PARAMETER_C:
                    withBase = false;
                    break;
                default:
                    return false;
            }
            int i;
            for (i = 2; i < args.Length && args[i] != "-path"; i++)
            {
                if (!string.IsNullOrWhiteSpace(args[i]))
                {
                    collections.Add(args[i]);
                }
            }
            if (!withBase && collections.Count < 1)
                return false;

            if (i++ < args.Length)
            {
                if (i >= args.Length)
                    return false;
                destPath = args[i];
                while (destPath.EndsWith("\\"))
                    destPath = destPath.Remove(destPath.Length - 2);
                if (string.IsNullOrWhiteSpace(destPath))
                    return false;
            }

            if (string.IsNullOrWhiteSpace(destPath))
                destPath = Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.LastIndexOf("\\"));

            return true;
        }
        static string InstallFiles(string destFolder, string appName, List<string> collections, bool withBase)
        {
            //string newtonsoft_package_framework = "portable46-win81+wpa81";
            //string newtonsoft_package_framework_path = "portable-net45+wp80+win8+wpa81+dnxcore50";
            string newtonsoft_package_version = "7.0.1";
            string search_newtonsoft_in_project_json_file = @"(\""Newtonsoft\.Json\"": \"")[^\""]*(\"")";
            string replace_newtonsoft_in_project_json_file = string.Format("${{1}}{0}$2", newtonsoft_package_version);
            //string search_newtonsoft_in_package_file = @"(.*id=\""Newtonsoft\.Json\"" version=\"")[^\""]*(\"" targetFramework=\"")[^\""]*(\"".*)";
            //string replace_newtonsoft_in_package_file = string.Format("${{1}}{0}${{2}}{1}$3", newtonsoft_package_version, newtonsoft_package_framework);
            //string search_newtonsoft_in_proj_file = @"(.*Newtonsoft\.Json\.)[^\\\\]*(\\lib\\)[^\\\\]*(.*)";
            //string replace_newtonsoft_in_proj_file = string.Format("${{1}}{0}${{2}}{1}$3", newtonsoft_package_version, newtonsoft_package_framework_path);
            string replace_alchiweb_modified = "////////////////////////////////////////\n// Modified by CategoriesForAppStudio //\n////////////////////////////////////////\n// CategoriesForAppStudio             //\n// Created by Hervé PHILIPPE          //\n// alchiweb@live.fr / alchiweb.fr     //\n////////////////////////////////////////\n\n";

            string success_base_install = "Success! All base files were successfully modified.\n";
            string success_collection_install = "Success! All files for the collection \"{0}\" were successfully modified.\n";
            string error_install_file = "Error. \"{0}\" file was not successfully {1}.\n";
            string fatal_error_install_file = "Fatal error.\n";
            string error_finding_collection = "Error finding collection with the config name file \"{0}\".\n";
            string result = "";
            string result_collection = "";
            string error_base_already_updated = "Error: base files are already updated.\n";
            string error_collection_already_updated = "Error: the files for the collection \"{0}\" are already updated.\n";
            FileToInstall[] base_files_to_install = {
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\CategoriesManager.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\ICategories.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\IDataProviderWithCategories.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\IHierarchical.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\IParserWithCategories.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Json\JsonDataConfig.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Json\JsonDataProvider.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Json\JsonDataProviderWithCategories.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Json\JsonHttpRequest.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Json\JsonHttpRequestResult.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Json\JsonHttpRequestSettings.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Json\JsonSchema.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Json\JsonSchemaWithCategories.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Json\Parser\JsonParser.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Json\Parser\JsonParserWithCategories.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Prestashop\PrestashopSchema.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Prestashop\Parser\PrestashopParser.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Wordpress\WordpressSchema.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\AppStudio.DataProviders\Wordpress\Parser\WordpressParser.cs"),
														 
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\ViewModels\DetailViewModelWithCategories.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\ViewModels\ListViewModelWithCategories.cs"),
                                                         new FileToUpdate(String.Format(@"{0}.W10\ViewModels\ItemViewModel.cs", appName), new NameValueCollection{
                                                             { @"^(.*)",
                                                                 replace_alchiweb_modified + "using AppStudio.DataProviders;\n$1"},
                                                             { @"(.*ISyncItem<ItemViewModel>)(.*)",
                                                                 "$1, IHierarchical$2"},
                                                             {@"(.*private string _content;)(.*)",
                                                                 "$1\n        private string _parent_id;$2"},
                                                             {@"(.*public string Id \{ get; set; \})(.*)",
                                                                 "$1\n\n        public string ParentId\n        {\n            get { return _parent_id; }\n            set { SetProperty(ref _parent_id, value); }\n        }\n$2"},
                                                             {@"(.* \|\| this\.Content \!= other\.Content)(.*)",
                                                                 "$1 || this.ParentId != other.ParentId$2"},
                                                             {@"(.*this\.Content = other.Content;)(.*)",
                                                                 "$1\n            this.ParentId = other.ParentId;$2"}
                                                                                                          }),
                                                         //new FileToUpdate(String.Format(@"{0}.W10\ViewModels\ShellViewModel.cs", appName), new NameValueCollection{
                                                             //{ @"^(.*)",
                                                                 //replace_alchiweb_modified + "$1"},
                                                             //{@"(.*)(OnPropertyChanged\(\""GoBackCommand\""\);.*)",
                                                                 //"$1var parameter_item = e.Parameter as ItemViewModel;\n            if (parameter_item != null && parameter_item.PageTitle != null)\n                AppTitle = parameter_item.PageTitle;\n            $2"} }),
                                                         new FileToUpdate(String.Format(@"{0}.W10\project.json", appName), new NameValueCollection{
                                                             { search_newtonsoft_in_project_json_file, replace_newtonsoft_in_project_json_file } }),

                                                         new FileToUpdate(String.Format(@"{0}.W10\{0}.W10.csproj", appName), new NameValueCollection{
                                                             {@"(.*<Compile Include=\""Properties\\AssemblyInfo\.cs\"" \/>)(.*)",
                                                                 "$1\n    <Compile Include=\"ViewModels\\DetailViewModelWithCategories.cs\" />\n    <Compile Include=\"ViewModels\\ListViewModelWithCategories.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\CategoriesManager.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\ICategories.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\IDataProviderWithCategories.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\IHierarchical.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\IParserWithCategories.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Json\\JsonDataConfig.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Json\\JsonDataProvider.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Json\\JsonDataProviderWithCategories.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Json\\JsonHttpRequest.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Json\\JsonHttpRequestResult.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Json\\JsonHttpRequestSettings.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Json\\JsonSchema.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Json\\JsonSchemaWithCategories.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Json\\Parser\\JsonParser.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Json\\Parser\\JsonParserWithCategories.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Prestashop\\PrestashopSchema.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Prestashop\\Parser\\PrestashopParser.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Wordpress\\WordpressSchema.cs\" />\n    <Compile Include=\"AppStudio.DataProviders\\Wordpress\\Parser\\WordpressParser.cs\" />$2"},
                                                                 }),
            };
            FileToInstall[] collection_files_to_install = {
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\Sections\[COLLECTION_CONFIG_NAME]Config - Prestashop.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\Sections\[COLLECTION_CONFIG_NAME]Config - Wordpress - CustomPost.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\Sections\[COLLECTION_CONFIG_NAME]Config - Wordpress.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\Sections\[COLLECTION_SCHEMA_NAME]Schema - Prestashop.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\Sections\[COLLECTION_SCHEMA_NAME]Schema - Wordpress - CustomPost.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\Sections\[COLLECTION_SCHEMA_NAME]Schema - Wordpress.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\Sections\[COLLECTION_CONFIG_NAME]WordpressParser - CustomPost.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\Sections\[COLLECTION_CONFIG_NAME]WordpressParser - CustomField.cs"),
                                                         new FileToUpdate(@"[WAS_APP_NAME].W10\ViewModels\MainViewModel.cs", new NameValueCollection{
                                                             { @"^(.*)",
                                                                 "using AppStudio.DataProviders.Json;\n$1"},
                                                             { @"(.* new ListViewModel)[^>]*([COLLECTION_SCHEMA_NAME]Schema>.*)",
                                                                "$1WithCategories<JsonDataConfig, $2"},
                                                            {@"(.*public ListViewModel)[^>]*([COLLECTION_SCHEMA_NAME]Schema>.*)",
                                                                "$1WithCategories<JsonDataConfig, $2"}
                                                            }),
                                                         new FileToUpdate(@"[WAS_APP_NAME].W10\Views\[COLLECTION_CONFIG_NAME]DetailPage.xaml.cs", new NameValueCollection{
                                                             { @"^(.*)",
                                                                 "using AppStudio.DataProviders.Json;\n$1"},
                                                             { @"(.* new DetailViewModel)[^>]*([COLLECTION_SCHEMA_NAME]Schema>.*)",
                                                                "$1WithCategories<JsonDataConfig, $2"},
                                                            {@"(.*public DetailViewModel)[^>]*([COLLECTION_SCHEMA_NAME]Schema>.*)",
                                                                "$1WithCategories<JsonDataConfig, $2"}
                                                            }),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\Views\[COLLECTION_CONFIG_NAME]ListPage.xaml.cs"),
                                                         new FileToUpdate(@"[WAS_APP_NAME].W10\Views\[COLLECTION_CONFIG_NAME]ListPage.xaml", new NameValueCollection{
                                                            //{@"(.*<Setter Target=\""bottomAppBar\.Visibility\"" Value=\""Collapsed\""\/>)(.*)",
                                                            //    "$1\n                        <Setter Target=\"topAppBar.Visibility\" Value=\"Collapsed\"/>\n                        <Setter Target=\"mainPanel.Margin\" Value=\"0,48,0,0\"/>$2"},
                                                            //{@"(.*<Setter Target=\""appBar\.\(Grid.Row\)\"" Value=\"")5(.*)",
                                                            //    "${1}6$2"},
                                                            {@"(.*)(<RowDefinition Height=\""\*\""\/>.*)",
                                                                "$1<RowDefinition Height=\"Auto\"/>\n                $2"},
                                                            {@"(.*)(<list_layouts:[^ ]* [^>]*Grid\.Row=\"")3([^>]*>[\n|\r|\r\n| ]*.*Grid\.Row=\"")4",
                                                                "$1<list_layouts:MenuText Grid.Row=\"3\" Grid.ColumnSpan=\"2\" DataContext=\"{Binding ViewModel}\" ItemsSource=\"{Binding Categories}\" ItemClickCommand=\"{Binding ItemClickCommand}\" OneRowModeEnabled=\"False\"/>\n            ${2}4${3}5"}
                                                            }),
                                                         new FileToUpdate(@"[WAS_APP_NAME].W10\[WAS_APP_NAME].W10.csproj", new NameValueCollection{
                                                             {@"(.*Include=""Sections\\[COLLECTION_CONFIG_NAME]Config)(.cs"".*)",
                                                                 "$1 - Prestashop$2"},
                                                             {@"(.*Include=""Sections\\[COLLECTION_SCHEMA_NAME]Schema)(.cs"".*)",
                                                                 "$1 - Prestashop$2"},
                                                         }),
            };

            if (withBase)
            {
                bool already_updated = File.Exists(destFolder + String.Format(@"\{0}.W10\AppStudio.DataProviders\CategoriesManager.cs", appName));
                if (!already_updated)
                {
                    foreach (var file in base_files_to_install)
                    {
                        if (file is FileToCopy)
                        {
                            (file as FileToCopy).SearchReplace.Add("[WAS_APP_NAME]", appName);
                        }
                        if (!file.Install(destFolder))
                            result += string.Format(error_install_file, file.Name, file is FileToCopy ? "copied" : "modified");
                    }
                    if (string.IsNullOrEmpty(result))
                        result = success_base_install;
                }
                else
                    result += error_base_already_updated;
            }
            if (collections != null)

                foreach (var collection_config_name in collections)
                {
                    Match result_regex = null;
                    bool already_updated = false;
                    string collection_schema_name = collection_config_name;
                    string collection_name = collection_config_name;
                    try
                    {
                        string file_txt = File.ReadAllText(string.Format(@"{0}\{1}.W10\Views\{2}DetailPage.xaml.cs", destFolder, appName, collection_config_name));
                        already_updated = Regex.IsMatch(file_txt, ".*DetailViewModelWithCategories.*");
                        if (!already_updated)
                            result_regex = Regex.Match(File.ReadAllText(String.Format(@"{0}\{1}.W10\Sections\{2}Config.cs", destFolder, appName, collection_config_name)),
                                @".*new DetailPageConfig<([^>]*)Schema>[\n|\r|\r\n| ]*\{[\n|\r|\r\n| ]*Title = \""([^\""]*)\""");
                    }
                    catch (Exception)
                    {
                        result_regex = null;
                    }
                    if (result_regex == null || !result_regex.Success || result_regex.Groups.Count < 3)
                    {
                        if (already_updated)
                            result_collection += string.Format(error_collection_already_updated, collection_config_name);
                        else
                            result_collection += string.Format(error_finding_collection, collection_config_name);
                    }
                    else
                    {
                        collection_schema_name = result_regex.Groups[1].Value;
                        collection_name = result_regex.Groups[2].Value;
                        foreach (var file in collection_files_to_install)
                        {
                            FileToInstall file_for_collection;
                            FileToCopy origin_copy_file = (file as FileToCopy);

                            if (origin_copy_file != null)
                            {
                                NameValueCollection new_search_replace = new NameValueCollection();
                                new_search_replace.Add("[WAS_APP_NAME]", appName);
                                new_search_replace.Add("[COLLECTION_CONFIG_NAME]", collection_config_name);
                                new_search_replace.Add("[COLLECTION_SCHEMA_NAME]", collection_schema_name);
                                new_search_replace.Add("[COLLECTION_NAME]", collection_name);
                                origin_copy_file.SearchReplace = new_search_replace;
                                file_for_collection = origin_copy_file;
                            }
                            else
                            {
                                FileToUpdate origin_replace_file = (file as FileToUpdate);
                                if (origin_replace_file != null)
                                {
                                    NameValueCollection search_replace = origin_replace_file.SearchReplace;
                                    if (search_replace != null && search_replace.Count > 0)
                                    {
                                        search_replace = new NameValueCollection();
                                        foreach (string search in origin_replace_file.SearchReplace)
                                        {
                                            search_replace.Add(
                                                search.Replace("[COLLECTION_CONFIG_NAME]", collection_config_name).Replace("[COLLECTION_SCHEMA_NAME]", collection_schema_name),
                                                origin_replace_file.SearchReplace[search].Replace("[COLLECTION_CONFIG_NAME]", collection_config_name).Replace("[COLLECTION_SCHEMA_NAME]", collection_schema_name)
                                                );
                                        }
                                    }

                                    file_for_collection = new FileToUpdate(
                                        file.Name.Replace("[WAS_APP_NAME]", appName).Replace("[COLLECTION_CONFIG_NAME]", collection_config_name).Replace("[COLLECTION_SCHEMA_NAME]", collection_schema_name),
                                        search_replace
                                        );
                                }
                                else
                                    file_for_collection = file;
                            }
                            if (file_for_collection == null)
                                result_collection += fatal_error_install_file;
                            else
                            {
                                if (!file_for_collection.Install(destFolder))
                                    result_collection += string.Format(error_install_file, file_for_collection.Name, file_for_collection is FileToCopy ? "copied" : "modified");
                            }
                        }
                    }
                    result += string.IsNullOrEmpty(result_collection) ? string.Format(success_collection_install, collection_config_name) : result_collection;
                }




            return "\n" + result;
        }
    }
}
