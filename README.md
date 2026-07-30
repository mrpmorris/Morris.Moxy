# Morris.Moxy
![](./Images/logo-small.png)

[![NuGet version](https://img.shields.io/nuget/v/Morris.Moxy.svg)](https://www.nuget.org/packages/Morris.Moxy)
[![NuGet downloads](https://img.shields.io/nuget/dt/Morris.Moxy.svg)](https://www.nuget.org/packages/Morris.Moxy)

***Morris.Moxy*** is a C# mix-in code generator for [Microsoft .NET](https://dotnet.microsoft.com/).

## Introduction
Moxy lets you write reusable template-based code in `.mixin` files and have it generated into your target classes at compile-time via Roslyn source generators.

It is designed to reduce repeated boilerplate by letting you define a pattern once, then apply that pattern to many classes using generated attributes.

## Goal
1. Write a code pattern once in a mixin template.
2. Let Moxy generate an attribute for that mixin.
3. Apply the generated attribute to classes/structs.
4. Get strongly-typed generated C# mixed into your partial types.

## Quick start
Install `Morris.Moxy` and reference it as an analyzer:

```xml
<ItemGroup>
  <PackageReference Include="Morris.Moxy" Version="*" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
</ItemGroup>
```

Add your mixin files as `AdditionalFiles`:

```xml
<ItemGroup>
  <AdditionalFiles Include="Mixins\SayHello.mixin" />
</ItemGroup>
```

## Example 1: Mix in a method
Create `Mixins/SayHello.mixin`:

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

Use the generated attribute on a partial class:

```c#
[SayHello]
internal partial class Person
{
}
```

Then call the mixed-in method:

```c#
var person = new Person();
person.SayHello();
```

## Example 2: Mix in with attribute arguments
Create `Mixins/CountDown.mixin`:

```c#
@moxy
@attribute required int From
@moxy

namespace {{ moxy.Class.Namespace }}
{
    partial class {{ moxy.Class.Name }}
    {
      public async Task StartCountDown(CancellationToken cancellationToken = default)
      {
        for(int i = {{ From }}; i >= 0; i--)
        {
            Console.WriteLine($"T minus {i}");
            await Task.Delay(1000, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
              return;
        }
      }
    }
}
```

Use it with arguments:

```c#
[CountDown(3)]
internal partial class MissionControl
{
}
```

## Viewing generated code
To inspect generated files, enable:

```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
```

Then use “Go to definition” (F12) on generated members (for example `SayHello()`).

## Getting started
The easiest way to get started is to read the [documentation](./Docs/README.md).
It includes tutorials in a recommended order, each with a `README` explaining the steps.

## Installation
You can download the latest release / pre-release NuGet package from:

* [Morris.Moxy](https://www.nuget.org/packages/Morris.Moxy) [![NuGet version (Morris.Moxy)](https://img.shields.io/nuget/v/Morris.Moxy.svg?style=flat-square)](https://www.nuget.org/packages/Morris.Moxy/)

## Release notes
See the [Releases page](./Docs/releases.md) for release history.

# Licence
[MIT](https://opensource.org/licenses/MIT)
