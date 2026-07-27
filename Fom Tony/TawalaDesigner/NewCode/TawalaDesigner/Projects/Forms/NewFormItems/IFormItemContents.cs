// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.Projects.Forms.NewFormItems
{
	public interface IFormItemContents 
	{
		string ToXml();
		string ToXhtml(IFormItem formItem);
		FormItemContentsCollection GetDescendants(Type descendantType);
		IFormItemContents Contents { get; set; }
		void ApplyFontStyle(FontStyle style);
		FontStyle GetInnermostFontStyle();
		string Text { get; }
		void ResolveFieldReferences();
		void ResolveFunctionReferences();
	}
}
