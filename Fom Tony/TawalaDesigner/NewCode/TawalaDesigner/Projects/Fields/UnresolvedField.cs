// $Workfile: UnresolvedField.cs $
// $Revision: 4 $	$Date: 2/28/08 5:30p $
// Copyright © 2005 - 2007 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects
{
	[Serializable]
	public class UnresolvedPaletteField : PaletteField
	{
		private string fullFieldName = "";

		public UnresolvedPaletteField(string fullFieldName)
		{
			this.fullFieldName = fullFieldName;
		}

		public override string QualifiedFieldName
		{
			get
			{
				return fullFieldName;
			}
		}

		public override string FieldName
		{
			get
			{
				return FieldUtil.GetFieldName(fullFieldName);
			}
		}

		public override IField this[string name]
		{
			get
			{
				return this;
			}
		}
	}

	[Serializable]
	public class UnresolvedField : Field, IDeserializedField
	{
		public UnresolvedField(string name)
		{
			this.name = name;
		}

		public override IField this[string name]
		{
			get
			{
				return this;
			}
		}

		#region IDeserializedField Members

		public IDeserializedField DeserializedFieldReference
		{
			get
			{
				return (IDeserializedField)this;
			}
		}

		#endregion
	}
}
