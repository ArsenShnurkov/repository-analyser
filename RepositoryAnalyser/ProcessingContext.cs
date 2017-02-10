using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace RepositoryAnalyser
{
	public class ProcessingContext
	{
		IndentedTextWriter writer;

		public IndentedTextWriter Writer
		{
			get
			{
				return writer;
			}
		}

		public SortedDictionary<string, string> ReferenceCount { get; }

		public ProcessingContext(IndentedTextWriter writer)
		{
			this.writer = writer;
			this.ReferenceCount = new SortedDictionary<string, string>();
		}
	}
}
