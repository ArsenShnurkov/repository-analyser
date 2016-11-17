using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using Mono.Options;

namespace RepositoryAnalyser
{
	partial class MainClass
	{
		public static void Main(string[] args)
		{
			var ops = new OptionSet()
			{
				{ "h|?|help", v => ShowHelp() },
			};
			List<string> unparsed = ops.Parse(args);
			if (unparsed.Count == 0)
			{
				ShowHelp();
			}
			using (var writer = new IndentedTextWriter(Console.Out))
			{
				foreach (string dir in unparsed)
				{
					var ctx = new ProcessingContext(writer);
					ProcessDirectory(dir, ctx);
				}
			}
		}
		public static void ShowHelp()
		{
			Console.WriteLine("Usage: ");
			Console.WriteLine("\trepository-analyser /var/calculate/remote/distfiles/egit-src/repository-analyser.git");
			Console.WriteLine("\trepository-analyser -h");
			Environment.Exit(0);
		}
	}
}
