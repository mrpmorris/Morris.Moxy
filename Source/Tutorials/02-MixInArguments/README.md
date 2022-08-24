# Moxy - Basic concepts

## Provide arguments to the mixin

### Referencing Moxy
1. Create a new console application
2. Add a NuGet package reference to `Morris.Moxy`
3. Edit the csproj file for your console application
4. Change the package reference to `Morris.Moxy` to add the following XML attribute values
    - OutputItemType="Analyzer"
    - ReferenceOutputAssembly="false"

### Creating a mixin file
1. Create a folder to contain your templates, I like to use "Mixins".
2. Inside that folder, create a new text file, and name it `CountDown.mixin`
3. In the properties for that file, set `Build Action` to `C# analyzer additional file`
4. Enter the following text into the mixin file

```c#
@moxy
@attribute required int From
@moxy

namespace {{ moxy.Class.Namespace }};

partial class {{ moxy.Class.Name }}
{
  public async Task StartCountDown(CancellationToken cancellationToken = default)
  {
    for(int i = {{ From }}; i >= 0; i--)
    {
        Console.WriteLine(string.Format("T minus {0}", i));
        await Task.Delay(1000, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
          return;
    }
  }
}
```

### Using the mixin file
1. Create a new class named `MissionControl.cs`
2. Add the auto-generated `[CountDown]` attribute to the `MissionControl` class

```c#
[CountDown(From: 3)]
internal class Person
{
}
```

3. In `Program.cs` add the following lines of code

```c#
Console.WriteLine("Press enter to quit");
var missionControl = new MissionControl();
_ = missionControl.StartCountDown();
Console.ReadLine();
```

4. Run the application and observe the output

### Viewing the generated code
1. Edit the csproj file
2. In a `<PropertyGroup>` add the following

```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
```

3. Go to `Program.cs`
4. Click on the `StartCountDown` in `missionControl.StartCountDown()`
5. Either right-click and select "Go to definition", or press F12

`MixInAMethod.Person.SayHello.Moxy.g.cs`
```c#
// Generated from Mixins\CountDown.mixin at 2022-08-24 21:03:08 UTC

namespace MixInArguments;

partial class MissionControl
{
  public async Task StartCountDown(CancellationToken cancellationToken = default)
  {
    for(int i = 3; i >= 0; i--)
    {
        Console.WriteLine(string.Format("T minus {0}", i));
        await Task.Delay(1000, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
          return;
    }
  }
}
```
