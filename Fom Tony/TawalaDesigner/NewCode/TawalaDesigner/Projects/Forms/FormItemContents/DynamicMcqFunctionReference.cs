// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.XmlSupport;
using Tawala.Common;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public class DynamicMcqFunctionReference : FunctionReference, IChoice
	{
		public DynamicMcqFunctionReference(IFunction function)
			: base(function)
		{
		}

		protected DynamicMcqFunctionReference(IXmlElement element)
			: base(element)
		{
		}

		protected DynamicMcqFunctionReference()
		{
		}

		#region IChoice Members

		public string ContentsXhtml(IFormItem formItem)
		{
			return ToXhtml(formItem);
		}

		public string ToXml(string label)
		{
			throw new NotImplementedException();
		}

		public string ChoiceLabel
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		#endregion

		#region IChoice Members


		public new string Text
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IField Members

		public string FieldName
		{
			get { throw new NotImplementedException(); }
		}

		public string FieldString
		{
			get { throw new NotImplementedException(); }
		}

		public IField this[string name]
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IRecursiveEnumerable Members

		public System.Collections.IEnumerable RecursiveEnumerator
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IAnyField Members

		public int Id
		{
			get { throw new NotImplementedException(); }
		}

		public string QualifiedFieldName
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

	[Serializable]
	public class DynamicMcqFunctionXmlReference : DynamicMcqFunctionReference
	{
		public DynamicMcqFunctionXmlReference(IXmlElement element)
			: base(element)
		{
		}
	}
}