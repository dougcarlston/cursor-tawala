// $Workfile: RecordField.cs $
// $Revision: 20 $	$Date: 6/18/07 5:51p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using System.Collections;

namespace Tawala.Projects
{
	/// <summary>
	/// A subclass of Field that represents a particular field in a record
	/// </summary>
	
	[Serializable]
	public class RecordField : Field, IAssignableField, IDeserializedField
	{
		public RecordField(Record record, IPaletteField referenceField) : this(record, referenceField, true)
		{
		}

		public RecordField(Record record, IPaletteField referenceField, bool addToFieldMap)
		{
			this.referenceField = referenceField;
			this.record = record;

			if (addToFieldMap)
			{
				Project.FieldMapById.AddUnique(this);
			}
		}

		public IDeserializedField DeserializedFieldReference
		{
			get
			{
				RecordField mappedField =  (RecordField)Project.FieldMapById[this.Id];

				return (new RecordField(mappedField.Record, (IPaletteField)mappedField.ReferenceField));
			}
		}

		public override string ToString()
		{
			if (referenceField == null)
			{
				return record.FieldName + ":" + base.ToString();
			}
			else
			{
				if (FieldUtil.IsUnknownField(referenceField.QualifiedFieldName))
				{
					return FieldUtil.UnknownFieldName;
				}
				return record.FieldName + ":" + referenceField.QualifiedFieldName;
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
					return "<<" + record.FieldName + ":" + name + ">>";
				}
				else
				{
					return "<<" + record.FieldName + ":" + referenceField.QualifiedFieldName + ">>";
				}
			}
		}

		public override string FieldName
		{
			get
			{
				if (referenceField == null)
				{
					return record.FieldName + ":" + name;
				}
				else
				{
					return record.FieldName + ":" + referenceField.FieldName;
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
					return record.FieldName + ":" + referenceField.QualifiedFieldName;
				}
			}
		}

		// field's Record variable
		private Record record;

		public Record Record
		{
			get
			{
				return record;
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

		#region IAssignable Members

		public string AssignmentName
		{
			get { return QualifiedFieldName; }
		}

		#endregion
	}
}
