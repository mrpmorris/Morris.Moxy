using Scriban;

namespace Morris.Moxy.Templates;

public readonly struct CompiledTemplate
{
	public readonly string Name;
	public readonly string FilePath;
	public readonly ParsedTemplate? Directives;
	public readonly Template? Template;

	public CompiledTemplate(
		string name,
		string filePath)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		Directives = null;
		Template = null;
	}

	public CompiledTemplate(
		string name,
		string filePath,
		ParsedTemplate? directives,
		Template? template)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		Directives = directives;
		Template = template;
	}
}
