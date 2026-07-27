using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects
{
	public class ProjectMissingDataSourcesException : Exception
	{
		private List<string> missingDataSourceNames = new List<string>();

		public ProjectMissingDataSourcesException()
		{
		}

		public List<string> MissingDataSourceNames
		{
			get { return missingDataSourceNames; }
		}
	}
}
