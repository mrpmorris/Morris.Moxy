using Morris.Moxy.Classes;
using Morris.Moxy.Extensions;
using Morris.Moxy.Helpers;
using Morris.Moxy.Templates;
using System.Collections.Immutable;

namespace Morris.Moxy;

internal readonly struct GeneratorInput
{
	public string? ProjectPath { get; init; }
	public string? RootNamespace { get; init;  }
	public ImmutableArray<ValidatedResult<CompiledTemplate>> ParsedTemplateResults { get; init; }
	public ImmutableArray<ClassInfo> ClassInfos { get; init; }

	public GeneratorInput() { }
	public GeneratorInput(GeneratorInput other)
	{
		ProjectPath = other.ProjectPath;
		RootNamespace = other.RootNamespace;
		ParsedTemplateResults = other.ParsedTemplateResults;
		ClassInfos = other.ClassInfos;
	}

	public override int GetHashCode() =>
		HashCode.Combine(ProjectPath, RootNamespace, ParsedTemplateResults.GetContentHashCode(), ClassInfos.GetContentHashCode());

	public override bool Equals(object? obj) =>
		obj is GeneratorInput other
		&& other.ProjectPath == ProjectPath
		&& other.RootNamespace == RootNamespace
		&& other.ParsedTemplateResults.SequenceEqual(ParsedTemplateResults)
		&& other.ClassInfos.SequenceEqual(ClassInfos);
}
