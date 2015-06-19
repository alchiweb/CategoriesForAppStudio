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
6. **Configure** the files in your `WAS_APP_NAME\Sections` directory, mainly the **`[COLLECTION_NAME]Config.cs`** file.<br/>
See further for more details.
7. **Build and run** your Windows 10 app (with Visual Studio 2015), and enjoy!!!!
 
# Where are your contents from?<br/>
For your app, your collections can use contents from:
- '**_static resources_**' (directly in your app)
- '**_dynamic resources_**':
  - public read access
  - managed by you, with your personal account in the Windows App Studio site
  - saved in the Microsoft Azure cloud
- an **online backoffice**<br/>
In this case, **CategoriesForAppStudio** can help you!<br/>
A cheap and easy way, is to use a backoffice solution with a REST API web service (and usually with categorized contents).<br/>
For example:
  - a CMS like Wordpress
  - an e-commerce solution like Prestashop

For another REST API, you will just have to create 2 classes (like for Wordpress and Prestashop), in the DataProviders project:
- **[YOUR_API]Schema.cs** file
- **[YOUR_API]Parser.cs** file
  
## Prestashop backoffice
### Configuring Prestashop site
Prestashop is provided with a REST API, disable by default.<br/>
In order to enable it (and to create the "**authentication key**", go to the [Prestashop documentation](http://doc.prestashop.com/display/PS14/Chapter+1+-+Creating+Access+to+Back+Office)).

You will have to check, at least:
- line **products**, column **GET**
- line **categories**, column **GET**

### Configuring your collection
The files to modify are in the 'WAS_APP_NAME' project, in the '**Sections**' directory.<br/>
By default, after the *CategoriesForAppStudio* installation, the Prestashop version of these files are used, but you can exclude them from the project and include the files from another version.

Note that the '*Show all files*' option in VS is very useful!).<br/>

2 files are used to configure your collection:

1. **`[COLLECTION_NAME]Shema - Prestashop.cs`**. This class must inherit from PrestashopSchema. No need to add something to this new class (unless you want to!).

2. **`[COLLECTION_NAME]Config - Prestashop.cs`**. This is the main config class.<br/>
For a quick configuration, all you need to do is:

- to replace the url with your own url, in the line:
```CSHARP
SiteUrl = "http://PRESTASHOP_SITE_NAME.com",
```
- to replace the "**API_KEY**" with your own "**authentication key**", in the line:
```CSHARP
NetCredential = new NetworkCredential("API_KEY", "", "")
```
  - to replace the "**category_2**" with your own root category id (in the example, the root category is "2"), in the line:
```CSHARP
new PrestashopParser<NewCol1Schema>("category_2"),
```
(null is allowed: this is the default value).

Note that you can change the following parameters, in the **JsonDataProviderWithCategories** constructor:
- the VisibleItemsType enum parameter: **All**, **CurrentLevel** and **AllForCurrentCategory**, to show the appropriate items
- the VisibleCategoriesType enum parameter: **All**, **CurrentLevel** and **NotEmpty**, to show the appropriate categories

## Wordpress backoffice
### Configuring Wordpress site
Wordpress doesn't yet provide a REST API, but will have it soon (in Wordpress v5?). The official plugin is [WP REST API](https://wordpress.org/plugins/json-rest-api/).<br/>
So you must install it in order to access to your contents through the API.

Notes:
- a [v2 version](http://v2.wp-api.org/) is coming, with big changes... but do not use it in production!
- [ACF (Advanced Custom Fields)](http://www.advancedcustomfields.com/) is a great plugin, that I used to:
  - add an image custom field to the categories,
  - add custom fields to a new custom post (like the standard "category" field)

### Configuring your collection
The files to modify are in the 'WAS_APP_NAME' project, in the '**Sections**' directory.<br/>
By default, after the *CategoriesForAppStudio* installation, the Prestashop version of these files are used, so you must exclude them from the project and include the files from another version.

Note that the '*Show all files*' option in VS is very useful!).<br/>

3 files are used to configure your collection:

1. **`[COLLECTION_NAME]Shema - Wordpress.cs`**. This class must inherit from WordpressSchema. No need to add something to this new class (unless you want to!).<br/>
For the CustomPost version, you can use the **`[COLLECTION_NAME]Shema - Wordpress - CustomPost.cs`** file, if you want...

2. **`[COLLECTION_NAME]WordpressParser - CustomField.cs`** (or **`[COLLECTION_NAME]WordpressParser - CustomPost.cs`** in the CustomPost case). This file is use to change the default behavior of the **WordpressParser**, in the case of custom fields.

3. **`[COLLECTION_NAME]Config.cs`**. This is the main config class.<br/>
For the CustomPost version, use the **`[COLLECTION_NAME]Config - Wordpress - CustomPost.cs`** file.<br/>
For a quick configuration, all you need to do is:

- to replace the url with your own url, in the line:
```CSHARP
SiteUrl = "http://WORDPRESS_SITE_NAME.com",
```
- for the custom post version, to replace the "**POSTTYPE_NAME**" with your own custom post name, in the lines:<br/>
```CSHARP
ApiFunction = "posts?type=POSTTYPE_NAME"
```
```CSHARP
new [COLLECTION_NAME]Parser<[COLLECTION_NAME]Schema>(null, "category", "POSTTYPE_NAME")
```
- to add your own root category id parameter (if needed), in the line:
```CSHARP
new [COLLECTION_NAME]Parser<[COLLECTION_NAME]Schema>(),
```

Note that you can add the following parameters, in the **JsonDataProviderWithCategories** constructor:
- the VisibleItemsType enum parameter: **All**, **CurrentLevel** and **AllForCurrentCategory**, to show the appropriate items
- the VisibleCategoriesType enum parameter: **All**, **CurrentLevel** and **NotEmpty**, to show the appropriate categories

Hervé PHILIPPE 
alchiweb@live.fr / http://alchiweb.fr