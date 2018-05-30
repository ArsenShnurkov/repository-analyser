using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.CodeDom.Compiler;
using CWDev.SLNTools.Core;
using System.Reflection;
using mptcore;
using BuildAutomation;

namespace RepositoryAnalyser
{
	partial class MainClass
	{
		static GacTools gac = new GacTools();
		static void ProcessSourcesRepository(string path, ProcessingContext ctx)
		{
			ctx.Writer.WriteLine("Processing sources repository: " + path);
			ctx.Writer.Indent++;
			try
			{
				var listOfFiles = Directory.EnumerateFiles(path, "*.sln", SearchOption.AllDirectories);
				foreach (string file in listOfFiles)
				{
					try
					{
						var sln = SolutionFile.FromFile(file);
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
			foreach (var item in ctx.Count)
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
			var fileName = new FileInfo(sln.SolutionFullPath).FullName; // Normalise name
			ctx.Writer.WriteLine("Processing solution: " + fileName);
			ctx.Writer.Indent++;
			try
			{
				foreach (var csproj in sln.Projects)
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
		static void ProcessProject(Project slnproj, ProcessingContext ctx)
		{
			var fileName = new FileInfo(slnproj.FullPath).FullName; // Normalise name
			ctx.Writer.WriteLine("Processing project: " + fileName);
			ctx.Writer.Indent++;
			try
			{
				if (slnproj.ProjectTypeGuid != KnownProjectTypeGuid.CSharp)
				{
					ctx.Writer.WriteLine($"unknown project type {slnproj.ProjectTypeGuid}");
					return;
				}
				var csproj = new ProjectAssemblyCSharp();
				csproj.Load(fileName);
				foreach (var asm in csproj.References)
				{
					var id = asm.AssemblyVersion.AssemblyName;
					string shortName = id;
					int index = id.IndexOf(",", StringComparison.InvariantCulture);
					if (index >= 0)
					{
						shortName = id.Substring(0, index);
					}
					if (ctx.Count.ContainsKey(id) || ctx.Count.ContainsKey(shortName))
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
					ctx.Count.Add(id, flag);
				}
				try
				{
					/*foreach (CSharpLibraryProject proj in csproj.Dependencies)
					{
						ctx.Writer.WriteLine("ProjectReference: " + proj.FileName);
					}*/
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
