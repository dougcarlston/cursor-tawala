// $Workfile: QualifiedFieldList.cs $
// $Revision: 10 $	$Date: 4/19/07 10:44a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Text;

namespace Tawala.Projects
{
	[Serializable]
	public class QualifiedFieldList : IPaletteField, IOperatorDataSource, IDeserializedField
	{
		public QualifiedFieldList()
		{
		}

		public QualifiedFieldList(Record qualifier, IField fields)
		{
			this.qualifier = qualifier;
			this.fields = fields;
		}

		public IDeserializedField DeserializedFieldReference
		{
			get
			{
				return (IDeserializedField)this;
			}
		}

		protected Record qualifier;

		private IField fields;

		public IField Fields
		{
			get
			{
				return fields;
			}
		}

		public override string ToString()
		{
			return FieldName;
		}

		#region IField Interface

		public string FieldName
		{
			get
			{
				string fieldName = "";

				foreach (IField field in fields.RecursiveEnumerator)
				{
					fieldName = qualifier.FieldName + ":" + field.FieldName;
				}

				return fieldName;
			}
		}

		public string QualifiedFieldName
		{
			get
			{
				return FieldName;
			}
		}

		public string FieldString
		{
			get
			{
				return "<<" + FieldName + ">>";
			}
		}

		public IField this[string name]
		{
			get
			{
				if (FieldName == name)
				{
					return this;
				}

				return null;
			}
		}

		private int id= Project.NextUniqueID;

		public int Id
		{
			get { return id; }
		}

		#endregion

		#region IEnumerable Interface

		public IEnumerator GetEnumerator()
		{
			foreach (IField field in fields)
			{
				yield return new Variable(qualifier.FieldName + ":" + field.FieldName);
			}
		}

		#endregion

		#region IRecursiveEnumerable Interface

		public IEnumerable RecursiveEnumerator
		{
			get
			{
				foreach (IPaletteField field in fields.RecursiveEnumerator)
				{
					yield return new RecordField(qualifier, field, false);
				}
			}
		}

		#endregion

		#region IOperatorDataSource

		public IList OperatorDataSource
		{
			get
			{
				return ((IOperatorDataSource)fields).OperatorDataSource;
			}
		}

		#endregion

	}
}
