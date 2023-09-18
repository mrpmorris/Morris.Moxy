# Releases

## New in 1.8
* Fix: When `ImplicitUsings` is disabled for a project, the attribute for a MixIn won't compile [Fixes #36](https://github.com/mrpmorris/Morris.Moxy/issues/36). 

## New in 1.7
* Fix: Moxy stops generating code when @moxy header is malformed [Fixes #33](https://github.com/mrpmorris/Morris.Moxy/issues/33)

## New in 1.6
* Honour Roslyn `ProductionContext.Cancellation.IsCancellationRequested` to improve performance
* Do not output `typeof()` in generated source when user has specified a type name as a mixin argument [Fixes #28](https://github.com/mrpmorris/Morris.Moxy/issues/28)
* Use object instead of string for variable values [Fixes #30](https://github.com/mrpmorris/Morris.Moxy/issues/30)

## New in 1.5
* Fixed IndexOutOfBoundsException when using attribute optional inputs [Fixes #19](https://github.com/mrpmorris/Morris.Moxy/issues/19)
* Static and aliased using clauses were not being output correctly [Fixes #20](https://github.com/mrpmorris/Morris.Moxy/issues/20)

## New in 1.4
* Allow mixins to be applied to structs [Fixes #13](https://github.com/mrpmorris/Morris.Moxy/issues/13)

## New in 1.3.1
* Rewritten to improve performance

## New in 1.2
* Allow mixins to be applied to a class more than once [Fixes #10](https://github.com/mrpmorris/Morris.Moxy/issues/10)

## New in 1.1
* Ensure the built-in functions like regex are not null - [Fixes #6](https://github.com/mrpmorris/Morris.Moxy/issues/6)

## New in 1.0
* First release

