using System;

namespace ResxDesignerGenerator
{
	class MainClass
	{
		public static int Main(string[] args)
		{
			if (args == null || args.Length < 4)
			{
				Console.WriteLine("Usage: ");
				Console.WriteLine(" ResxDesignerGenerator <resxfile> <namespace> <classname> <outputfile>");
				return 1;
			}
			
			ResXFileCodeGenerator.GenerateDesignerFile(args[0], args[1], args[2], args[3]);
			return 0;
		}
	}
}
