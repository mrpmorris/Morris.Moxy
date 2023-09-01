# Page mix-in

## Purpose
To create compile-time checked route definitions for Blazor pages.

## Installing Moxy
1. Create a new Blazor application
2. Add a NuGet package reference to `Morris.Moxy`
3. Edit the csproj file for your console application
4. Change the package reference to `Morris.Moxy` to add the following XML attribute values
    - OutputItemType="Analyzer"
    - ReferenceOutputAssembly="false"

## Import this mixin
1. Create a folder to contain your templates, I like to use "Mixins".
2. Inside that folder, create a new text file, and name it `MixInPage.mixin`
3. In the properties for that file, set `Build Action` to `C# analyzer additional file`
4. Enter the text from `Page.mixin` into the file

## Usage
1. Go to your .razor file and remove the `@Page` directive.
2. Create a page-behind file `YourPage.razor.cs` (Note: Roslyn only works on .cs files)
3. Add the `[MixInPage]` attribute to your code-behind class, like so
```c#
[MixInPage("/company/{CompanyId:Guid}/employee/{EmployeeNumber:int}
public partial class EmployeeViewPage
{
}
```

## Result
From anywhere in your app you can now obtain a fully formatted URL like so

```c#
YourPage.GetPageUrl(TheCompanyGuid, TheEmployeeNumber);
```



