using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.CodeDom.Compiler;
using MetaSpecTools;
using System.Reflection;
using mptcore;

namespace RepositoryAnalyser
{
	partial class MainClass
	{
		static GacTools gac = new GacTools();
		static void ProcessDirectory(string path, ProcessingContext ctx)
		{
			var rep = new Repository(path);
			ctx.Writer.WriteLine("Processing directory: " + path);
			ctx.Writer.Indent++;
			try
			{
				var listOfFiles = Directory.EnumerateFiles(path, "*" + SolutionFile.DefaultExtension, SearchOption.AllDirectories);
				foreach (string file in listOfFiles)
				{
					try
					{
						var sln = SolutionFile.FromFile(file, rep);
						ProcessSolution(sln, ctx);
					}
					catch (Exception ex)
					{
						ctx.Writer.Write("Error while processing solution: " + ex.ToString());
					}
				}
			}
			finally
			{
				ctx.Writer.Indent--;
			}
			ctx.Writer.WriteLine("Summary:");
			var libs = new SortedList<string,bool>();
			foreach (var item in ctx.ReferenceCount)
			{
				libs.Add($"{item.Value} {item.Key}", false);
			}
			foreach (var item in libs.Keys)
			{
				ctx.Writer.WriteLine($"{item}");
			}
		}
		static void ProcessSolution(SolutionFile sln, ProcessingContext ctx)
		{
			var fileName = new FileInfo(sln.SolutionFullName).FullName; // Normalise name
			ctx.Writer.WriteLine("Processing solution: " + fileName);
			ctx.Writer.Indent++;
			try
			{
				if (sln.Projects.Count == 0)
				{
					Debugger.Break();
				}
				foreach (Project csproj in sln.Projects)
				{
					try
					{
						ProcessProject(csproj, ctx);
					}
					catch (Exception ex)
					{
						ctx.Writer.WriteLine("Exception: " + ex.ToString());
					}
				}
			}
			finally
			{
				ctx.Writer.Indent--;
			}
		}
		static void ProcessProject(Project csproj, ProcessingContext ctx)
		{
			var fileName = new FileInfo(csproj.FullPath).FullName; // Normalise name
			ctx.Writer.WriteLine("Processing project: " + fileName);
			ctx.Writer.Indent++;
			try
			{
				foreach (ReferencedAssembly asm in csproj.References)
				{
					var id = asm.AssemblyName;
					string shortName = id;
					int index = id.IndexOf(",");
					if (index >= 0)
					{
						shortName = id.Substring(0, index);
					}
					if (ctx.ReferenceCount.ContainsKey(id) || ctx.ReferenceCount.ContainsKey(shortName))
					{
						continue;
					}
					string flag = string.Empty;

					if (gac.IsAssemblyInGAC(shortName))
					{
						flag = "[~]";
						if (id != shortName && gac.IsAssemblyInGAC(id))
						{
							flag = "[=]";
						}
						else
						{
							id = shortName;
						}
					}
					ctx.Writer.WriteLine("Reference: " + id);
					ctx.ReferenceCount.Add(id, flag);
				}
				try
				{
					foreach (Project proj in csproj.Dependencies)
					{
						ctx.Writer.WriteLine("ProjectReference: " + proj.FullPath);
					}
				}
				catch (Exception ex)
				{
					ctx.Writer.WriteLine("Exception: " + ex.ToString());
				}
			}
			finally
			{
				ctx.Writer.Indent--;
			}
		}
	}
}
