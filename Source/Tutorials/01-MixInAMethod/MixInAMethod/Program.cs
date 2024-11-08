// See https://aka.ms/new-console-template for more information

using MixInAMethod;

System.Console.WriteLine("Hello, World!");

var person = new Person();
person.SayHello();
person.ShowMoxyVariables();
person.DumpMembers();

var nested = new Container.NestedClass();
nested.ShowMoxyVariables();