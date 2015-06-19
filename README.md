# What is **CategoriesForAppStudio**?
**CategoriesForAppStudio** modify your [*Windows App Studio*](http://appstudio.windows.com) Visual Studio solution, in order to:
 - use generic categories for your items collections, show them and navigate through categories
 - easily connect to a Rest Api (Json or Xml)
 - easily connect to a Cms Api. Example provided for Wordpress and Prestashop.
 
# How to use it?
1. **Generate** an application with [*Windows App Studio*](http://appstudio.windows.com), that contains one (or more) collection(s)
2. **Download and unzip** the Visual Studio source package
3. **Copy [Install_CategoriesForAppStudio directory](Install_CategoriesForAppStudio)** to your root directory VS solution
4. **Launch `cmd.exe`** (the command line utility) and go to the Install_CategoriesForAppStudio directory in your VS solution
5. **Launch CategoriesForAppStudio**:<br/>
  `CategoriesForAppStudio {WAS_APP_NAME} -bc|-c {NAME_COL_CONFIG_1} {NAME_COL_CONFIG_2} ... {NAME_COL_CONFIG_N} [-path {PATH_DEST}]` where:
  - **`WAS_APP_NAME`** is the name of the application (used to the namespace, without spaces or other special characters)
  - use **`-bc`** to generate the base and eventually the collection(s)
  - use **`-c`** to only generate the collection(s)
  - **`NAME_COL_CONFIG`** is the name of the collection config class (= collection config .cs file)
  - use **`-path PATH_DEST`** to specify the root directory of the VS solution (if the Install_CategoriesForAppStudio directory isn't in the root directory of the VS solution)
6. **Configure** the files in your `WAS_APP_NAME\Sections` directory, mainly the **`*Config.cs`** files.
 By default, the Prestashop version of these files are used, but you can exclude these files from the project and include an other version (thanks to the '*Show all files*' option in VS!). See further for more details.
7. **Build and run** your Windows 10 app (with Visual Studio 2015), and enjoy!!!!
 
# Where are your contents from?<br/>
For your app, your collections can use contents from:
- '*static resources*' (directly in your app)
- '*dynamic resources*':
  - public read access
  - managed by you, with your personal account in the Windows App Studio site
  - saved in the Microsoft Azure cloud
- an online backoffice:<br/>
in this case, **CategoriesForAppStudio** can help you!<br/>
A cheap and easy way, is to use a backoffice solution with a REST API web service (and usually with categorized contents).<br/>
For example:
- a CMS like Wordpress
- an e-commerce solution like Prestashop

For another REST API, you will just have to create 2 classes (like for Wordpress and Prestashop), in the DataProviders project:
- [API]Schema.cs file
- [API]Parser.cs file
  
## Prestashop backoffice
### Configuring Prestashop site
Prestashop is provided with a REST API, disable by default.<br/>
In order to enable it (and to create the "**authentication key**", go to the [Prestashop documentation](http://doc.prestashop.com/display/PS14/Chapter+1+-+Creating+Access+to+Back+Office)

You will have to check, at least:
- "**products**" with GET
- "**categories**" with GET

### Configuring your collection

## Wordpress backoffice
### Configuring Wordpress site
### Configuring your collection

Hervé PHILIPPE 
alchiweb@live.fr / http://alchiweb.fr