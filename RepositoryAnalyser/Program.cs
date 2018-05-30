namespace RepositoryAnalyser
{
	using System;
	using System.IO;
	using System.Configuration;
	using System.Collections.Generic;
	using System.CodeDom.Compiler;
	using Mono.Options;
	using Mono.TextTemplating;
	using mptcore;
	using System.Linq;
	using BuildAutomation;
	using Ebuild;

	partial class MainClass
	{
		public static int Main(string[] args)
		{
			//string configurationDirectory = ConfigurationManager.AppSettings["configurationDirectory"];

			var suite = new CommandSet ("repository-analyzer")
			{
				{ "h|?|help", v => ShowHelp() },
				new Command ("git-url", "downloads repository from URI and analyze it") {
					Options = new OptionSet {/*...*/},
					Run = GitUrl,
				},
				new Command ("git-dir", "analyze the given directory") {
					Options = new OptionSet {/*...*/},
					Run = GitDir,
				},
				new Command ("convert", "command help") {
					Options = new OptionSet {/*...*/},
					Run = Convert,
				},
			};
			return suite.Run (args);
		}

		public static void GitUrl(IEnumerable<string> enumerable_arguments)
		{
			var args = Enumerable.ToArray(enumerable_arguments);
			if (args.Length == 0)
			{
				ShowHelp();
				return;
			}
			Console.WriteLine("GitUrl");
		}

		public static void GitDir(IEnumerable<string> enumerable_arguments)
		{
			var args = Enumerable.ToArray(enumerable_arguments);
			if (args.Length == 0)
			{
				ShowHelp();
				return;
			}
			using (var writer = new IndentedTextWriter(Console.Out))
			{
				foreach (string dir in args)
				{
					var ctx = new ProcessingContext(writer);
					ProcessSourcesRepository(dir, ctx);
					GenerateMakefile(dir);
					GenerateEbuild(dir);
				}
			}
		}

		public static void Convert(IEnumerable<string> enumerable_arguments)
		{
			var args = Enumerable.ToArray(enumerable_arguments);
			if (args.Length != 2)
			{
				ShowHelp();
				return;
			}
			string inputName = args[0];
			string outputName = args[1];
			var pa = new ProjectAssemblyCSharp();
			pa.Load(inputName);
			var eb = new EbuildDocument();
			Convert(pa,eb);
			eb.SaveTo(outputName);
		}
		public static void Convert(ProjectAssemblyCSharp src, EbuildDocument dst)
		{
		}
		public static void ShowHelp()
		{
			Console.WriteLine("Usage: ");
			Console.WriteLine("\trepository-analyser /var/calculate/remote/distfiles/egit-src/repository-analyser.git");
			Console.WriteLine("\trepository-analyser -h");
			Environment.Exit(0);
		}
		public static void GenerateMakefile(string dir)
		{
			var di = new DirectoryInfo(dir);
			var output_dir = string.Format("/var/tmp/{0}", di.Name);
			if (Directory.Exists(output_dir) == false)
			{
				Directory.CreateDirectory(output_dir);
			}
			string outputFile = String.Format("{0}/Makefile", output_dir);
			var generator = new SessionTemplateGenerator ();
			// Create a Session in which to pass parameters:
			generator.Session = generator.CreateSession();
			generator.Session["Model"] = "TestModelString";
			string inputFile = "ebuild-tempate.t4";
			generator.ProcessTemplate(inputFile, outputFile);
		}
		public static void GenerateEbuild(string dir)
		{
			var di = new DirectoryInfo(dir);
			var output_dir = string.Format($"/var/tmp/{di.Name}");
			if (Directory.Exists(output_dir) == false)
			{
				Directory.CreateDirectory(output_dir);
			}
			string outputFile = String.Format($"{output_dir}/{di.Name}-1.0.0.0.ebuild");
			var generator = new SessionTemplateGenerator ();
			// Create a Session in which to pass parameters:
			generator.Session = generator.CreateSession();
			generator.Session["CurrentYear"] = DateTime.Now.Year;
			generator.Session["Model"] = "TestModelString";
			string inputFile = "ebuild-tempate.t4";
			generator.ProcessTemplate(inputFile, outputFile);
		}
	}
}
