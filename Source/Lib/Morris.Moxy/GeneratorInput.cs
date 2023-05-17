using Morris.Moxy.Classes;
using Morris.Moxy.Extensions;
using Morris.Moxy.Helpers;
using Morris.Moxy.Templates;
using System.Collections.Immutable;

namespace Morris.Moxy;

internal readonly struct GeneratorInput : IEquatable<GeneratorInput>
{
	public string? ProjectPath { get; }
	public string? RootNamespace { get; }
	public ImmutableArray<ValidatedResult<CompiledTemplate>>? ParsedTemplateResults { get; }
	public ImmutableArray<ClassInfo>? ClassInfos { get; }
	
	private readonly Lazy<int> CachedHashCode;

	public GeneratorInput(
		string? projectPath = null,
		string? rootNamespace = null,
		ImmutableArray<ValidatedResult<CompiledTemplate>>? parsedTemplateResults = null,
		ImmutableArray<ClassInfo>? classInfos = null)
		: this(
			other: null,
			projectPath: projectPath,
			rootNamespace: rootNamespace,
			parsedTemplateResults: parsedTemplateResults,
			classInfos:	classInfos)
	{
	}

	private GeneratorInput(
		GeneratorInput? other,
		string? projectPath = null,
		string? rootNamespace = null,
		ImmutableArray<ValidatedResult<CompiledTemplate>>? parsedTemplateResults = null,
		ImmutableArray<ClassInfo>? classInfos = null)
	{
		projectPath ??= other?.ProjectPath;
		rootNamespace ??= other?.RootNamespace;
		parsedTemplateResults ??= other?.ParsedTemplateResults;
		classInfos ??= other?.ClassInfos;

		ProjectPath = projectPath;
		RootNamespace = rootNamespace;
		ParsedTemplateResults = parsedTemplateResults;
		ClassInfos = classInfos;
		CachedHashCode = new Lazy<int>(() =>
			HashCode.Combine(
				projectPath,
				rootNamespace,
				parsedTemplateResults?.GetContentHashCode(),
				classInfos?.GetContentHashCode()));
	}

	public GeneratorInput WithProjectPath(string? projectPath) => new GeneratorInput(this, projectPath: projectPath);

	public GeneratorInput WithRootNamespace(string? rootNamespace) => new GeneratorInput(this, rootNamespace: rootNamespace);

	public GeneratorInput WithParsedTemplateResults(ImmutableArray<ValidatedResult<CompiledTemplate>> parsedTemplateResults) => new GeneratorInput(this, parsedTemplateResults: parsedTemplateResults);

	public GeneratorInput WithClassInfos(ImmutableArray<ClassInfo> classInfos) => new GeneratorInput(this, classInfos: classInfos);

	public override bool Equals(object obj) =>
		obj is GeneratorInput other && Equals(other);

	public bool Equals(GeneratorInput other) =>
		ProjectPath == other.ProjectPath
		&& RootNamespace == other.RootNamespace
		&& ParsedTemplateResults?.GetContentHashCode() == other.ParsedTemplateResults?.GetContentHashCode()
		&& ClassInfos?.GetContentHashCode() == other.ClassInfos?.GetContentHashCode();

	public override int GetHashCode() => CachedHashCode.Value;

	public static bool operator ==(GeneratorInput left, GeneratorInput right) => left.Equals(right);

	public static bool operator !=(GeneratorInput left, GeneratorInput right) => !(left == right);
}


