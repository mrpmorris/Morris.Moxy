# Moxy - Basic concepts

## Mix in a method

### Referencing Moxy
1. Create a new console application
2. Add a NuGet package reference to `Morris.Moxy`
3. Edit the csproj file for your console application
4. Change the package reference to `Morris.Moxy` to add the following XML attribute values
    - OutputItemType="Analyzer"
    - ReferenceOutputAssembly="false"

### Creating a mixin file
1. Create a folder to contain your templates, I like to use "Mixins".
2. Inside that folder, create a new text file, and name it `SayHello.mixin`
3. In the properties for that file, set `Build Action` to `C# analyzer additional file`
4. Enter the following text into the mixin file

```c#
namespace {{ moxy.Class.Namespace }}
{
    partial class {{ moxy.Class.Name }}
    {
      public void SayHello()
      {
        Console.WriteLine("Hello");
      }
    }
}
```

### Using the mixin file
1. Create a new class named `Person.cs`
2. Add the auto-generated `[SayHello]` attribute to the `Person` class

```c#
[SayHello]
internal class Person
{
}
```

3. In `Program.cs` add the following lines of code

```c#
var person = new Person();
person.SayHello();
```

4. Run the application and observe the output

### Viewing the generated code
1. Edit the csproj file
2. In a `<PropertyGroup>` add the following

```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
```

3. Go to `Program.cs`
4. Click on the `SayHello()` in `person.SayHello`
5. Either right-click and select "Go to definition", or press F12

`MixInAMethod.Person.Moxy.g.cs`
```c#
// Generated from mixin: Mixins\SayHello.mixin
namespace MixInAMethod
{
    partial class Person
    {
      public void SayHello()
      {
        Console.WriteLine("Hello");
      }
    }
}
```
