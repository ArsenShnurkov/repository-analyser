using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace RepositoryAnalyser
{
	public class ProcessingContext
	{
		public ProcessingContext(IndentedTextWriter writer)
		{
			this.writer = writer;
			this.Count = new SortedDictionary<string, string>();
		}
		IndentedTextWriter writer;
		public IndentedTextWriter Writer
		{
			get
			{
				return writer;
			}
		}
		public SortedDictionary<string, string> Count { get; }
	}
}

