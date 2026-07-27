using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects.Forms
{
	public interface IFormItem : IField
	{
		IForm Form { get; set; }
		bool IsTextItem { get; }
		bool IsQuestionItem { get; }
		string AlternateLabel { get; set; }
		bool Selected { get; set; }
		string Style { get; set; }
		Conditions DisplayConditions { get; set; }
		bool HasDisplayConditions { get; }
		void ClearId();
		void Eliminate();
		string ToXml();
		void ResolveFieldReferences();
		void ResolveFunctionReferences();
	}
}
