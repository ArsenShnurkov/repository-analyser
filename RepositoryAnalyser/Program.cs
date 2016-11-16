using System;
using System.Collections.Generic;
using Mono.Options;

namespace RepositoryAnalyser
{
	class MainClass
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
			foreach (string url in unparsed)
			{
				ProcessUrl(url);
			}
		}
		static void ProcessUrl(string url)
		{
			Console.WriteLine(url);
		}
		public static void ShowHelp()
		{
			Console.WriteLine("Usage: ");
			Console.WriteLine("\trepository-analyser http://github.com/Arsen.Shnurkov/repository-analyser");
			Console.WriteLine("\trepository-analyser -h");
			Environment.Exit(0);
		}
	}
}
