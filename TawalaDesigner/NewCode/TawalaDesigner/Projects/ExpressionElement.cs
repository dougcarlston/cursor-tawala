// $Workfile: ExpressionElement.cs $
// $Revision: 14 $	$Date: 2/28/08 5:30p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements an element of an expression
	/// </summary>
	
	[Serializable]
	public class ExpressionElement
	{
		public ExpressionElement()
		{
		}

		public ExpressionElement(string elementString)
		{
		}

		public virtual string Text
		{
			get { return String.Empty; }
		}

		public virtual string ToXml()
		{
			return "";
		}
	}

	/// <summary>
	/// Implements a field element of an expression
	/// </summary>
	[Serializable]
	public class FieldElement : ExpressionElement
	{
		public FieldElement(IField field)
		{
			this.field = (IDeserializedField)field;
		}

		public override string ToString()
		{
			return Field.FieldString;
		}

		private IDeserializedField field;

		public IDeserializedField Field
		{
			get
			{
				if (field is UnresolvedField)
				{
					field = FieldUtil.ResolveFormField(field) as IDeserializedField;
				}

				return field;
			}
			set
			{
				field = value;
			}
		}

		public override string Text
		{
			get
			{
				return Field.QualifiedFieldName;
			}
		}

		protected const string xmlFieldNameTag = "<field name=\"{0}\"/>";

		public override string ToXml()
		{
			return String.Format(xmlFieldNameTag, Text);
		}

		[OnDeserialized]
		private void onDeserialized(StreamingContext context)
		{
			field = Field.DeserializedFieldReference;
		}
	}

	/// <summary>
	/// Implements a string element of an expression
	/// </summary>
	[Serializable]
	public class StringElement : ExpressionElement
	{
		public StringElement(string elementString)
		{
			this.elementString = elementString;
		}

		public override string ToString()
		{
			return elementString;
		}

		private string elementString;

		public override string Text
		{
			get
			{
				return elementString;
			}
		}

		public override string ToXml()
		{
			return Text;
		}

		[OnDeserialized]
		private void onDeserialized(StreamingContext context)
		{
			// REVISIT: This code was put in place specifically to cause an exception to be thrown when opening
			// projects created by Build 29. It may become unnecessary in future builds.
			if (elementString == null)
			{
				throw new SerializationException();
			}
		}


	}
}
