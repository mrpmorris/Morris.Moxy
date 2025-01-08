using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MixInArguments;

[AutoInterface("ISomeClass")]
public class SomeClass
{
	public string Name { get; set; }
	public string Description { get; }
	public int Add(int x,int y) => x + y;
	public int Add10OrMore(int x, int y = 10) => x + Math.Min(10, y);
}
