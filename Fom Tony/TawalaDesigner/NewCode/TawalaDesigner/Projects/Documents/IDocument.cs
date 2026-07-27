using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Documents
{
	public interface IDocument : IComponent
	{
		FieldList GetFields();
		IFormItemContents NewContents { get; set; }
	}
}
