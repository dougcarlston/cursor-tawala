using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Interfaces;

namespace Tawala.DocumentDesigner
{
	public interface ITawalaDocumentDesigner : IProjectComponentDesigner
	{
		IDocumentView CurrentDocumentView
		{
			get;
			set;
		}
	}
}
