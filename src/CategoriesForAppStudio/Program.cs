using System;
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
            string newtonsoft_package_framework = "portable46-win81+wpa81";
            string newtonsoft_package_framework_path = "portable-net45+wp80+win8+wpa81+dnxcore50";
            string newtonsoft_package_version = "7.0.1-beta3";
            string search_newtonsoft_in_package_file = @"(.*id=\""Newtonsoft\.Json\"" version=\"")[^\""]*(\"" targetFramework=\"")[^\""]*(\"".*)";
            string replace_newtonsoft_in_package_file = string.Format("${{1}}{0}${{2}}{1}$3", newtonsoft_package_version, newtonsoft_package_framework);
            string search_newtonsoft_in_proj_file = @"(.*Newtonsoft\.Json\.)[^\\\\]*(\\lib\\)[^\\\\]*(.*)";
            string replace_newtonsoft_in_proj_file = string.Format("${{1}}{0}${{2}}{1}$3", newtonsoft_package_version, newtonsoft_package_framework_path);
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
                                                         new FileToCopy(@"AppStudio.DataProviders\CategoriesManager.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\ICategories.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\IDataProviderWithCategories.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\IHierarchical.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\IParserWithCategories.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\JsonClient\JsonDataConfig.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\JsonClient\JsonDataProvider.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\JsonClient\JsonDataProviderWithCategories.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\JsonClient\JsonInternetRequest.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\JsonClient\JsonSchema.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\JsonClient\JsonSchemaWithCategories.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\JsonClient\Parser\JsonParser.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\JsonClient\Parser\JsonParserWithCategories.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\Prestashop\PrestashopSchema.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\Prestashop\Parser\PrestashopParser.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\Wordpress\WordpressSchema.cs"),
                                                         new FileToCopy(@"AppStudio.DataProviders\Wordpress\Parser\WordpressParser.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME]\ViewModels\DetailViewModelWithCategories.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME]\ViewModels\ListViewModelWithCategories.cs"),
                                                         new FileToUpdate(String.Format(@"{0}\ViewModels\ItemViewModel.cs", appName), new NameValueCollection{
                                                             { @"^(.*)",
                                                                 replace_alchiweb_modified + "using AppStudio.DataProviders;\n$1"},
                                                             { @"(.*ISyncItem<ItemViewModel>)(.*)",
                                                                 "$1, IHierarchical$2"},
                                                             {@"(.*private string _content;)(.*)",
                                                                 "$1\n        private string _parent_id;$2"},
                                                             {@"(.*public string _id \{ get; set; \})(.*)",
                                                                 "$1\n\n        public string ParentId\n        {\n            get { return _parent_id; }\n            set { SetProperty(ref _parent_id, value); }\n        }\n$2"},
                                                             {@"(.* \|\| this\.Content \!= other\.Content)(.*)",
                                                                 "$1 || this.ParentId != other.ParentId$2"},
                                                             {@"(.*this\.Content = other.Content;)(.*)",
                                                                 "$1\n            this.ParentId = other.ParentId;$2"}
                                                                                                          }),
                                                         new FileToUpdate(String.Format(@"{0}.W10\ViewModels\ShellViewModel.cs", appName), new NameValueCollection{
                                                             { @"^(.*)",
                                                                 replace_alchiweb_modified + "$1"},
                                                             {@"(.*)(OnPropertyChanged\(\""GoBackCommand\""\);.*)",
                                                                 "$1var parameter_item = parameter as ItemViewModel;\n            if (parameter_item != null && parameter_item.PageTitle != null)\n                AppTitle = parameter_item.PageTitle;\n            $2"} }),
                                                         new FileToUpdate(String.Format(@"{0}.W10\packages.config", appName), new NameValueCollection{
                                                             { search_newtonsoft_in_package_file, replace_newtonsoft_in_package_file } }),
                                                         new FileToUpdate(String.Format(@"{0}.W10\{0}.W10.csproj", appName), new NameValueCollection{
                                                             { search_newtonsoft_in_proj_file, replace_newtonsoft_in_proj_file },
                                                         }),
                                                         new FileToUpdate(String.Format(@"{0}.Windows\packages.config", appName), new NameValueCollection{
                                                             { search_newtonsoft_in_package_file, replace_newtonsoft_in_package_file } }),
                                                         new FileToUpdate(String.Format(@"{0}.Windows\{0}.Windows.csproj", appName), new NameValueCollection{
                                                             { search_newtonsoft_in_proj_file, replace_newtonsoft_in_proj_file },
                                                         }),
                                                         new FileToUpdate(@"AppStudio.Common\packages.config", new NameValueCollection{
                                                             { search_newtonsoft_in_package_file, replace_newtonsoft_in_package_file } }),
                                                         new FileToUpdate(@"AppStudio.Common\AppStudio.Common.csproj", new NameValueCollection{
                                                             { search_newtonsoft_in_proj_file, replace_newtonsoft_in_proj_file },
                                                         }),
                                                         new FileToUpdate(String.Format(@"{0}.WindowsPhone\packages.config", appName), new NameValueCollection{
                                                             { search_newtonsoft_in_package_file, replace_newtonsoft_in_package_file } }),
                                                         new FileToUpdate(String.Format(@"{0}.WindowsPhone\{0}.WindowsPhone.csproj", appName), new NameValueCollection{
                                                             { search_newtonsoft_in_proj_file, replace_newtonsoft_in_proj_file },
                                                         }),
                                                         new FileToUpdate(String.Format(@"{0}\packages.config", appName), new NameValueCollection{
                                                             { search_newtonsoft_in_package_file, replace_newtonsoft_in_package_file } }),
                                                         new FileToUpdate(String.Format(@"{0}\{0}.csproj", appName), new NameValueCollection{
                                                             { search_newtonsoft_in_proj_file, replace_newtonsoft_in_proj_file },
                                                             {@"(.*<Compile Include=\""Properties\\AssemblyInfo\.cs\"" \/>)(.*)",
                                                                 "$1\n    <Compile Include=\"ViewModels\\DetailViewModelWithCategories.cs\" />\n    <Compile Include=\"ViewModels\\ListViewModelWithCategories.cs\" />$2"},
                                                         }),
                                                         new FileToUpdate(@"AppStudio.DataProviders\packages.config", new NameValueCollection{
                                                             { search_newtonsoft_in_package_file, replace_newtonsoft_in_package_file } }),
                                                         new FileToUpdate(@"AppStudio.DataProviders\AppStudio.DataProviders.csproj", new NameValueCollection{
                                                             { search_newtonsoft_in_proj_file, replace_newtonsoft_in_proj_file },
                                                             {@"(.*<Compile Include=\""Properties\\AssemblyInfo\.cs\"" \/>)(.*)",
                                                                 "$1\n    <Compile Include=\"CategoriesManager.cs\" />\n    <Compile Include=\"ICategories.cs\" />\n    <Compile Include=\"IDataProviderWithCategories.cs\" />\n    <Compile Include=\"IHierarchical.cs\" />\n    <Compile Include=\"IParserWithCategories.cs\" />\n    <Compile Include=\"JsonClient\\JsonDataConfig.cs\" />\n    <Compile Include=\"JsonClient\\JsonDataProvider.cs\" />\n    <Compile Include=\"JsonClient\\JsonDataProviderWithCategories.cs\" />\n    <Compile Include=\"JsonClient\\JsonInternetRequest.cs\" />\n    <Compile Include=\"JsonClient\\JsonSchema.cs\" />\n    <Compile Include=\"JsonClient\\JsonSchemaWithCategories.cs\" />\n    <Compile Include=\"JsonClient\\Parser\\JsonParser.cs\" />\n    <Compile Include=\"JsonClient\\Parser\\JsonParserWithCategories.cs\" />\n    <Compile Include=\"Prestashop\\PrestashopSchema.cs\" />\n    <Compile Include=\"Prestashop\\Parser\\PrestashopParser.cs\" />\n    <Compile Include=\"Wordpress\\WordpressSchema.cs\" />\n    <Compile Include=\"Wordpress\\Parser\\WordpressParser.cs\" />$2"},
                                                         }),
            };
            FileToInstall[] collection_files_to_install = {
                                                         new FileToCopy(@"[WAS_APP_NAME]\Sections\[COLLECTION_CONFIG_NAME]Config - Prestashop.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME]\Sections\[COLLECTION_CONFIG_NAME]Config - Wordpress - CustomPost.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME]\Sections\[COLLECTION_CONFIG_NAME]Config - Wordpress.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME]\Sections\[COLLECTION_SCHEMA_NAME]Schema - Prestashop.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME]\Sections\[COLLECTION_SCHEMA_NAME]Schema - Wordpress - CustomPost.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME]\Sections\[COLLECTION_SCHEMA_NAME]Schema - Wordpress.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME]\Sections\[COLLECTION_CONFIG_NAME]WordpressParser - CustomPost.cs"),
                                                         new FileToCopy(@"[WAS_APP_NAME]\Sections\[COLLECTION_CONFIG_NAME]WordpressParser - CustomField.cs"),
                                                         new FileToUpdate(@"[WAS_APP_NAME]\ViewModels\MainViewModel.cs", new NameValueCollection{
                                                            {@"(.* new ListViewModel)(<[COLLECTION_SCHEMA_NAME]Schema>.*)",
                                                                "$1WithCategories$2"},
                                                            {@"(.*public ListViewModel)(<[COLLECTION_SCHEMA_NAME]Schema>.*)",
                                                                "$1WithCategories$2"}
                                                            }),
                                                         new FileToUpdate(@"[WAS_APP_NAME].Shared\Views\[COLLECTION_CONFIG_NAME]DetailPage.cs", new NameValueCollection{
                                                            {@"(.*new DetailViewModel)(.*)",
                                                                "$1WithCategories$2"},
                                                            {@"(.*public DetailViewModel)(.*)",
                                                                "$1WithCategories$2"}
                                                            }),
                                                         new FileToCopy(@"[WAS_APP_NAME].Shared\Views\[COLLECTION_CONFIG_NAME]ListPage.cs"),
                                                         new FileToUpdate(@"[WAS_APP_NAME].W10\Views\[COLLECTION_CONFIG_NAME]DetailPage.xaml.cs", new NameValueCollection{
                                                            {@"(.*new DetailViewModel)(.*)",
                                                                "$1WithCategories$2"},
                                                            {@"(.*public DetailViewModel)(.*)",
                                                                "$1WithCategories$2"}
                                                            }),
                                                         new FileToCopy(@"[WAS_APP_NAME].W10\Views\[COLLECTION_CONFIG_NAME]ListPage.xaml.cs"),
                                                         new FileToUpdate(@"[WAS_APP_NAME].W10\Views\[COLLECTION_CONFIG_NAME]ListPage.xaml", new NameValueCollection{
                                                            {@"(.*<Setter Target=\""bottomAppBar\.Visibility\"" Value=\""Collapsed\""\/>)(.*)",
                                                                "$1\n                        <Setter Target=\"topAppBar.Visibility\" Value=\"Collapsed\"/>\n                        <Setter Target=\"mainPanel.Margin\" Value=\"0,48,0,0\"/>$2"},
                                                            {@"(.*)(<RowDefinition Height=\""\*\""\/>.*)",
                                                                "$1<RowDefinition Height=\"Auto\"/>\n                $2"},
                                                            {@"(.*)(<layouts:ListLayout )(.*)",
                                                                "$1<layouts:ListLayout DataContext=\"{Binding}\" ItemsSource=\"{Binding Categories}\"\n                                HasLoadDataErrors=\"{Binding HasLoadDataErrors, FallbackValue=False}\"\n                                ItemClickCommand=\"{Binding ItemClickCommand}\" ListLayoutTemplate=\"MenuListPhoto\"/>\n            $2 Grid.Row=\"1\" $3"}
                                                            }),
                                                         new FileToUpdate(@"[WAS_APP_NAME]\[WAS_APP_NAME].csproj", new NameValueCollection{
                                                             {@"(.*Include=""Sections\\[COLLECTION_CONFIG_NAME]Config)(.cs"".*)",
                                                                 "$1 - Prestashop$2"},
                                                             {@"(.*Include=""Sections\\[COLLECTION_SCHEMA_NAME]Schema)(.cs"".*)",
                                                                 "$1 - Prestashop$2"},
                                                         }),
            };

            if (withBase)
            {
                bool already_updated = File.Exists(destFolder + @"\AppStudio.DataProviders\CategoriesManager.cs");
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
                            result_regex = Regex.Match(File.ReadAllText(String.Format(@"{0}\{1}\Sections\{2}Config.cs", destFolder, appName, collection_config_name)),
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
