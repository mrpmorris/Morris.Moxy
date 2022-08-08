using Scriban;

namespace Morris.Moxy.DataStructures;

public readonly struct CompiledTemplate
{
	public readonly string Name;
	public readonly string FilePath;
	public readonly string Source;
	public readonly ParsedTemplate Directives;
	public readonly Template Template;

	public CompiledTemplate(
		string name,
		string filePath,
		string source,
		ParsedTemplate directives,
		Template template)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		Source = source ?? throw new ArgumentNullException(nameof(source));
		Directives = directives;
		Template = template ?? throw new ArgumentNullException(nameof(template));
	}
}
