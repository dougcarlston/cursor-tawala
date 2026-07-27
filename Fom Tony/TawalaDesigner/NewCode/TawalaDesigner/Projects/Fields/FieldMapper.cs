using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Tawala.Projects
{
	/// <summary>
	/// Class to map text strings (such as "Q1:a" and "Record:Score") to the actual fields they represent.
	/// This class is used in converting XML elements to their respective Tawala objects.
	/// </summary>
	public class FieldMapper : Dictionary<string, IField>
	{
		private string parentName = "";

		public FieldMapper()
		{
		}

		public FieldMapper(string parentName)
		{
			this.parentName = parentName;
		}

		/// <summary>
		/// List of all fields, qualified and unqualified.
		/// </summary>
		private FieldList allFields = new FieldList();

		public FieldList AllFields
		{
			get
			{
				return allFields;
			}
		}

		/// <summary>
		/// List of fields, such as "Q1:a".
		/// </summary>
		private FieldList fields = new FieldList();

		public FieldList Fields
		{
			get
			{
				return fields;
			}
		}

		/// <summary>
		/// List of field qualifiers, such as "Record 1"
		/// </summary>
		private Collection<string> qualifiers = new Collection<string>();

		public Collection<string> Qualifiers
		{
			get
			{
				return qualifiers;
			}
		}

		/// <summary>
		/// Places all combinations of qualifiers and fields into the field mapper
		/// </summary>
		public void Map()
		{
			Clear();
			allFields.Clear();

			mapUnqualifiedFields();

			mapFormQualifiedFields();

			mapRecordQualifiedFields();
		}

		/// <summary>
		/// Places unqualified fields (e.g. "Q1:a") into the field mapper
		/// </summary>
		private void mapUnqualifiedFields()
		{
			foreach (IField field in fields.RecursiveEnumerator)
			{
				string key = field.FieldName;

				if (!this.ContainsKey(key))
				{
					this[key] = field;
					allFields.Add(field);
				}
			}
		}

		/// <summary>
		/// Places form-qualified fields (e.g. "Form1:Q1:a") into the field mapper
		/// </summary>
		private void mapFormQualifiedFields()
		{
			foreach (IField field in fields.RecursiveEnumerator)
			{
				string key = field.ToString();

				if (!this.ContainsKey(key))
				{
					this[key] = field;
					allFields.Add(field);
				}
			}
		}

		/// <summary>
		/// Places record-qualified fields (e.g. "Record1:Q1:a", "Record1:Form 1:Q1:a") into the field mapper
		/// </summary>
		private void mapRecordQualifiedFields()
		{
			foreach (string qualifier in qualifiers)
			{
				foreach (IPaletteField field in fields.RecursiveEnumerator)
				{
					string recordQualifiedKey = qualifier + ":" + field.FieldName;
					string recordAndFormQualifiedKey = qualifier + ":" + field.QualifiedFieldName;

					if (!this.ContainsKey(recordQualifiedKey))
					{
						RecordField qualifiedField = new RecordField(new Record(qualifier), field);
						this[recordQualifiedKey] = qualifiedField;
						allFields.Add(qualifiedField);
					}

					if (!this.ContainsKey(recordAndFormQualifiedKey))
					{
						RecordField qualifiedField = new RecordField(new Record(qualifier), field);
						this[recordAndFormQualifiedKey] = qualifiedField;
						allFields.Add(qualifiedField);
					}
				}
			}
		}

		public void Empty()
		{
			this.Clear();
			fields.Clear();
			allFields.Clear();
			qualifiers.Clear();
		}
	}
}
