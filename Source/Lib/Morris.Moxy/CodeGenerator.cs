using Microsoft.CodeAnalysis;

namespace Morris.Moxy
{
  [Generator]
  public class CodeGenerator : IIncrementalGenerator
  {
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
	  Console.Beep();
	}
  }
}
