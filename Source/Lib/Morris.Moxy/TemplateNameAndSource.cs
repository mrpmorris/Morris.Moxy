namespace Morris.Moxy.DataStructures
{
  public readonly record struct TemplateNameAndSource(string Name, string FilePath, string Source)
  {
	public static TemplateNameAndSource Create(string name, string filePath, string source)
	{
	  Console.Beep();
	  return new TemplateNameAndSource(name, filePath, source);
	}
  }
}