# What is **CategoriesForAppStudio**?
**CategoriesForAppStudio** modify your [*Windows App Studio*](http://appstudio.windows.com) Visual Studio solution, in order to:
 - use generic categories for your items collections, show them and navigate through categories
 - easily connect to a Rest Api (Json or Xml)
 - easily connect to a Cms Api. Example provided for Wordpress and Prestashop.
 
# How to use it?
 1. Generate an application with [*Windows App Studio*](http://appstudio.windows.com), that contains one (or more) collection(s)
 
 2. Download and unzip the Visual Studio source package
 
 3. Copy [Install_CategoriesForAppStudio directory](Install_CategoriesForAppStudio) to your root directory VS solution
 
 4. Launch the command line utility (Cmd.exe) ang go to the Install_CategoriesForAppStudio directory in your VS solution
 
 5. Launch **CategoriesForAppStudio** with the following parameters:
 
`>CategoriesForAppStudio {WAS_APP_NAME} -bc|-c {NAME_COL_CONFIG_1} {NAME_COL_CONFIG_2} ... {NAME_COL_CONFIG_N} [-path {PATH_DEST}]`
 where:
   - **`WAS_APP_NAME`** is the name of the application (used to the namespace, without spaces or other special characters)
   - use **`-bc`** to generate the base and eventually the collection(s)
   - use **`-c`** to only generate the collection(s)
   - **`NAME_COL_CONFIG`** is the name of the collection config class (= collection config .cs file)
   - use **`-path PATH_DEST`** to specify the root directory of the VS solution (if the Install_CategoriesForAppStudio directory isn't in the root directory of the VS solution)
*Note that the console app will tell you if errors occurs.*
   
6. Configure the files in your **`WAS_APP_NAME\Sections`** directory, mainly the **`*Config.cs`** files.
 By default, the Prestashop version of these files are used, but you can exclude these files from the project and include an other version (thanks to the 'Show all files' option in VS ;)!).
 
7. Build and run your Windows 10 app (with Visual Studio 2015), and enjoy!!!!

Hervé PHILIPPE 
alchiweb@live.fr / http://alchiweb.fr