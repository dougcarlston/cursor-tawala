// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.XmlSupport;
using Tawala.Common;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Function;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public abstract class FunctionReference : FormItemContents
	{
        [NonSerialized]
		private IXmlElement element;

		private int functionId;

		protected FunctionReference()
		{
		}

		protected FunctionReference(IXmlElement element)
		{
			this.element = element;
			Function = new XmlToFunctionConverter().ConvertFrom(element);
		}

		protected FunctionReference(IFunction function)
		{
			Function = function;
		}

		public override void ResolveFunctionReferences()
		{
			if (element != null)
			{
				Function = new XmlToFunctionConverter().ConvertFrom(element);
                element = null;
			}
		}

		public IFunction Function 
		{
            get { return Project.FunctionMapById[functionId]; }

			set
			{
				functionId = value.InstanceId;
                Project.FunctionMapById.AddUnique(value);
			}
		}

		#region IFormItemContents Members

		public override string ToXml()
		{
            return Function.ToXml();
		}

		public override string ToXhtml(IFormItem formItem)
		{
			return string.Format("<t:function id=\"func_{0}\">{1}</t:function>",
                Function.InstanceId,
				XMLStringFormatter.EscapeElementText(Function.ToDisplayString().Replace("<<", "").Replace(">>", "")));
		}

		public override string Text
		{
			get { return Function.ToDisplayString(); }
		}

		#endregion
	}

	[Serializable]
	public class FunctionXmlReference : FunctionReference
	{
		public FunctionXmlReference(IXmlElement element) : base(element)
		{
		}

	}

	[Serializable]
	public class FunctionXhtmlReference : FunctionReference
	{
		public FunctionXhtmlReference(IXmlElement element)
		{
			int id = Convert.ToInt32(element.GetAttribute("id").Replace("func_",""));
			Function = Project.FunctionMapById[id];
		}
	}
}
