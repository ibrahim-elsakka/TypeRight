# TypeRight
TypeRight is a simple tool that generates TypeScript files from your C# objects and controller actions. This project stemmed from some annoying things I found while doing web development using C# and MVC:

1. Making requests required entering a free text URL and parameter names.  If anything changed, the request broke.
2. If you wanted to strongly type a result from a web request, you would need to make the same C# object in TypeScript.  This led to maintenance nightmares trying to keep those in sync.
3. Using enums on the server and client required keeping them in sync

This tool solves those problems by autogenerating those TypeScript files every time you build your project.

# Quick Start

1. Find ["TypeRight" in Nuget](https://www.nuget.org/packages/TypeRight) and install it (generally to the web project, but it could be for any)
2. (Optional) If you haven't already, install the [VS Extension](https://marketplace.visualstudio.com/items?itemName=someguy20336.TypeRight) for a better experience.  This extension is not required for the whole thing to work, but it does add some benefits.
   - Better script generating performance since it integrates directly with the Visual Studio data model
   - Some helper commands to install the package, generate scripts on demand, and add the config file
   - Down the road, this extension may become more useful... but that is TBD
3. Add or update the typeRightConfig.json file.  As of 0.5.2, a default config file is included in the nuget content.  You can also add the config file via a right click menu option on the project node if you have extension installed.  Config options are located below.
   - **NOTE**: if you install TypeRight in multiple projects for your solution, the config file will be pulled into each one.  You should disable it through the config option for all projects that aren't web projects (i.e. your core/business class library projects).  It was a battle between making it easy to include the config and incorrectly having config files for projects that shouldn't.  Maybe I'll fix it someday...
4. Add the `ScriptObject` attribute to any classes or interfaces you want to extract to a TypeScript file
   - If you aren't a fan of polluting your business classes with attributes, you can use the assembly attribute `ExternalScriptObjectAttribute` to provide a list of types to extract.  Example: `[assembly: ExternalScriptObject(typeof(Class1), typeof(Class2), ...)]`
5. Add the `ScriptEnum` attribute to any enums you want to extract to a TypeScript file
6. Add the `ScriptAction` attribute to any Controller Actions (methods) that you want to extract to TypeScript files
7. Build the project.  The following TypeScript files will now be created
   - One containing all the classes, interfaces, and enums output to the location specified in the **serverObjectsResultFilepath** config option (default "./Scripts/ServerObjects.ts")
   - One for each controller with extracted actions. The output path follows the pattern `<project Root>\Scripts\<Controller Name>\<Controller Name>Actions.ts`


# Configuration
In order to properly function, a config file must exist in the web csproj root directory named "typeRightConfig.json". The config file consists of the following options:

- **enabled**
  - a boolean flag for whether script generation is enabled
- **templateType**
  - The type of template to use.  The options are "namespace" or "module".  More about these templates below, but "module" is recommended.
- **classNamespace**
  - When using the namespace template, this is the namespace to use for class/interface types
- **enumNamespace**
  - When using the namespace template, this is the namespace to use for enums
- **webMethodNamespace**
  - When using the namespace template, this is the namespace to use for controller actions (more about that below)
- **serverObjectsResultFilepath**
  - The relative path (from the perspective of the csproj root) to which the classes, interfaces, and enums should be printed out to.  By default, this value is "./Scripts/ServerObjects.ts"
- **ajaxFunctionName**
  - Allows you to define the function that makes Ajax calls.  More later in the Extracting Controller Actions section
- **ajaxFunctionModulePath**
  - The module file location for the ajax function defined above.  Used only in the module template.  Similiar to the server object result filepath, it should be relative from the perspective of the csproj root.

# Extracting classes, interfaces, and enums
Extracting a class or interface is as simple as adding an the `ScriptObject` attribute to the class.  From there, all properties and documentation are automatically extracted to a TypeScript file.  This output file location is configured by the `serverObjectsResultFilepath` configuration option.

As a example, the following C# object:

```C#

/// <summary>
/// This is my class
/// </summary>
[ScriptObject]
public class MyClass
{
  /// <summary>
  /// Gets the first property
  /// </summary>
  public string PropertyOne { get; }
  
  public int PropertyTwo { get; }
  
  public List<string> PropertyThree { get; }
}
```

will be extracted to the following TypeScript object

```TypeScript
/*
 * This is my class
 */
export interface MyClass {
  /** Gets the first property */
  PropertyOne: string;
  
  PropertyTwo: number;
  
  PropertyThree: string[];
}
```

The other alternative for classes is to use `ExternalScriptObjectAttribute`.  For this assembly level attribute, you can specify types that are in another project or DLL.  You might use this if you don't want to install the nuget package in your core project or referenced code from another DLL.  Here is how you might use it:

```C#
[assembly: ExternalScriptObject(typeof(Class1), typeof(Class2), ...)]
```

Enums are extracted by adding the `ScriptEnum` attribute.  This will create an enumeration type in TypeScript.  As another example:

```C#
[ScriptEnum]
public enum MyEnum 
{
  One = 1
  Two = 2,
  ThreeHundred = 300
}

```

will give you 

```TypeScript
export enum MyEnum {
  One = 1,
  Two = 2,
  ThreeHundred = 300
}
```

The `ScriptEnum` attribute has one property named `UseExtendedSyntax` that can be used to export the enum with some additional properties that are sometimes useful if your enum might have a display name or abbreviation.  Note, to add a display name or abbreviation, you can use any attribute that inherits from `IEnumDisplayNameProvider`.  A default implementation is provided in `EnumDisplayName`.  Due to the nature of parsing the code, **it is important that all properties be defined in the constructor as Named Arguments**.

```C#
[ScriptEnum(UseExtendedSyntax = true)]
public enum MyEnum 
{
  One = 1
  Two = 2,
  [DefaultEnumDisplayNameProvider(DisplayName = "Three Hundred", Abbreviation = "3 Hundo")]
  ThreeHundred = 300
}

```

will give you 

```TypeScript
export let MyEnum = {
  One = {
    id: 1,
    name: "One",
    abbrev: "One"
  },
  Two = {
    id: 2,
    name: "Two",
    abbrev: "Two"
  },
  ThreeHundred = {
    id: 300,
    name: "Three Hundred",
    abbrev: "3 Hundo"
  }
}
```

# Extracting Controller Actions
Since all of the `ScriptObject`s and `ScriptEnum`s will be objects that you are passing back and forth between web service calls, it would make sense to strongly type those web service, too.  Currently, TypeRight only works with MVC actions.  To create a script for MVC Actions, add the `ScriptAction` attribute to the method call.  The output for these types of scripts will be:

`<project Root>\Scripts\<Controller Name>\<Controller Name>Actions.ts`

Let's take a look at an example.  Say we have this controller:

```C#

public class MyDefaultController : Controller 
{
  public ActionResult Index() 
  {
    // Do things
  }
  
  [ScriptAction]
  public JsonResult GetSomeData(int param1, MyClass someOtherData)
  {
    // Do things
    
    return Json(new MySecondClass())
  }
}

```

Assuming `MyClass` and `MySecondClass` have been attributed with `ScriptObject`, the resulting TypeScript will be created (some template specific details have been omitted for clarity):

```TypeScript

export function GetSomeData(param1: number, 
                            someOtherData: MyClass, 
                            success?: (result: MySecondClass) => void, 
                            fail?: () => void): void {
      // Makes a POST webservce call to /MyDefault/GetSomeData      
      // How this is done is configurable 
}

```

Now here is where the configuration comes into play.  

- By default, each controller action will call an auto generated method that uses $.ajax (yes... JQuery.  Maybe I will change this some day)
- You can change this to your own custom function located somewhere in your project by using the **ajaxFunctionName** configuration setting.  The function must have the following signature: (url: string, data: any, success: (result: any) => void, fail: () => any): any
  - The url you get will be in the form: `/<Controller Name>/<Action Name>`.  If you use Areas, it will be `/<Area Name>/<Controller Name>/<Action>`
  - The data you get will be a dictionary of keys (parameter names) and values
- If using the module template, note that you will need to specify the file that the module is located in the **ajaxFunctionModulePath** configuration setting

You will notice that the `success` function will automatically pull the return type of the webservice call.  It does this by:
1. Finding the first return statement
2. If the return statement is the function "Json(...)", it will pull the type from the first parameter
3. Otherwise, it is whatever the return type of the method is


# Templates
There are two types of templates for no other reason than they are what I have used in the past.  If you are interested in some other type of template, let me know!  I think sometime in the future it would be cool to give users more flexibility on the template, but that is some ways off.

## Module Template
This is likely the recommended template with todays standards and module loading.  For this template, the result files will just be a collection of classes/methods that can be imported into other files.

## Namespace Template
This template is probably not the recommended one to use anymore, but I used it in the past so maybe someone might find it useful.  For this template, all classes/service calls are wrapped in a "namespace".  For example:

```TypeScript
namespace MyNamespace.Classes {
  export interface SomeType {
      // ... things
  }
}
```

```TypeScript
namespace MyNamespace.WebMethods {
  export function getStuff(param:string, success, fail): void {
    // Do stuff
  }
}
```

