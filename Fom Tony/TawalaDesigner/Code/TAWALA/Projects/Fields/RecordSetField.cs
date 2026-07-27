// $Workfile: RecordSetField.cs $
// $Revision: 4 $	$Date: 4/19/07 10:44a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using System.Collections;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;

namespace Tawala.Projects
{
	[Serializable]
	public class RecordSetField : Field, IOperatorDataSource, IDeserializedField
	{
		public RecordSetField(RecordSet recordSet, IPaletteField referenceField) : this(recordSet, referenceField, true)
		{
		}

		public RecordSetField(RecordSet recordSet, IPaletteField referenceField, bool addToFieldMap)
		{
			this.recordSet = recordSet;
			this.referenceField = referenceField;

			if (addToFieldMap)
			{
				Project.FieldMapById.AddUnique(this);
			}
		}

		public override string ToString()
		{
			if (referenceField == null)
			{
				return recordSet.FieldName + ":" + base.ToString();
			}
			else
			{
				return recordSet.FieldName + ":" + referenceField.QualifiedFieldName;
			}
		}

		public IDeserializedField DeserializedFieldReference
		{
			get
			{
				RecordSetField mappedField = (RecordSetField)Project.FieldMapById[this.Id];

				return (new RecordSetField(mappedField.RecordSet, (IPaletteField)mappedField.ReferenceField));
			}
		}

		/// <summary>
		/// Placed in documents, expressions, etc
		/// </summary>
		public override string FieldString
		{
			get
			{
				if (referenceField == null)
				{
					return "<<" + recordSet.FieldName + ":" + name + ">>";
				}
				else
				{
					return "<<" + recordSet.FieldName + ":" + referenceField.QualifiedFieldName + ">>";
				}
			}
		}

		public override string FieldName
		{
			get
			{
				if (referenceField == null)
				{
					return recordSet.FieldName + ":" + name;
				}
				else
				{
					return recordSet.FieldName + ":" + referenceField.FieldName;
				}
			}
		}

		public override string QualifiedFieldName
		{
			get
			{
				if (referenceField == null)
				{
					return FieldName;
				}
				else
				{
					return recordSet.FieldName + ":" + referenceField.QualifiedFieldName;
				}
			}
		}

		private RecordSet recordSet;

		public RecordSet RecordSet
		{
			get
			{
				return recordSet;
			}
		}

		private IPaletteField referenceField;

		public IField ReferenceField
		{
			get
			{
				return referenceField;
			}
		}

		public override IEnumerable RecursiveEnumerator
		{
			get
			{
				yield return this;
			}
		}


		#region IOperatorDataSource

		public override IList OperatorDataSource
		{
			get
			{
				return ((IOperatorDataSource)referenceField).OperatorDataSource;
			}
		}

		#endregion

	}
}
