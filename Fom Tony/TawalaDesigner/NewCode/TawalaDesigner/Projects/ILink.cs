using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects
{
	public interface ILink
	{
		int Id { get; }

		string DisplayText { get; set; }

		string ToXml();
	}
}
