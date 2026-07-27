using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.Projects.Forms.NewFormItems
{
	public interface IFibItem : IFormItem
	{
		Collection<IBlank> BlankList { get; }
		string ToXml(string label);
		void InsertBlanksIntoFieldMapByName();
	}
}
