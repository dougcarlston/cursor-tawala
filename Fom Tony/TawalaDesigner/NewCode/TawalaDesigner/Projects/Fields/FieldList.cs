// $Workfile: FieldList.cs $
// $Revision: 12 $	$Date: 1/14/08 6:30p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Forms;

namespace Tawala.Projects
{
	/// <summary>
	/// List of fields.
	/// </summary>
	[Serializable]
	public class FieldList : Collection<IField>, IField, IRecursiveEnumerable
	{
		public static FieldList NULL = new FieldList();
		private SortedList fieldsByName = new SortedList();

		private static int nextFieldListId = 0;

		public FieldList()
		{
			fieldListId = ++nextFieldListId;
		}

		/// <summary>
		/// Add field to end of list if field name does not already exist in list.
		/// </summary>
		/// <param name="field">Field to add</param>
		/// <returns>Index at which field was added</returns>
		public void AddUnique(IField field)
		{
			foreach (IField f in field.RecursiveEnumerator)
			{
				foreach (IField item in this.RecursiveEnumerator)
				{
					if (f.Id == item.Id)
					{
						return;
					}
				}
			}

			//Add(field);
			add(field);
		}

		public new void Add(IField item)
		{
			add(item);
		}

		protected override void ClearItems()
		{
			base.ClearItems();
			fieldsByName.Clear();
		}

		protected override void InsertItem(int index, IField item)
		{
			base.InsertItem(index, item);
			addToFieldsByName(item);
		}

		private void add(IField field)
		{
			base.Add(field);
		}

		//private void addToFieldsByName(IField field)
		//{
		//    if (field is IPaletteField)
		//    {
		//        if (field is FibItem)
		//        {
		//            FibItem fibItem = field as FibItem;

		//            foreach (IPaletteField subField in fibItem.RecursiveEnumerator)
		//            {
		//                fieldsByName[subField.QualifiedFieldName] = subField;
		//                fieldsByName[subField.FieldName] = subField;
		//            }
		//        }
		//        else if (field is QualifiedFieldList)
		//        {
		//            QualifiedFieldList qualifiedFieldList = field as QualifiedFieldList;

		//            foreach (IPaletteField subField in qualifiedFieldList.RecursiveEnumerator)
		//            {
		//                fieldsByName[subField.QualifiedFieldName] = subField;
		//                fieldsByName[subField.FieldName] = subField;
		//            }
		//        }
		//        else
		//        {
		//            fieldsByName[((IPaletteField)field).QualifiedFieldName] = field;
		//        }
		//    }

		//    fieldsByName[field.FieldName] = field;
		//}

		private void addToFieldsByName(IField field)
		{
			if (field is IPaletteField)
			{
				foreach (IPaletteField subField in field.RecursiveEnumerator)
				{
					fieldsByName[subField.QualifiedFieldName] = subField;
					fieldsByName[subField.FieldName] = subField;
				}

				fieldsByName[((IPaletteField)field).QualifiedFieldName] = field;
			}

			fieldsByName[field.FieldName] = field;
		}

		/// <summary>
		/// Tests whether this list contains all the members of another list
		/// </summary>
		/// <param name="otherList">Field list to compare</param>
		public bool ContainsAll(FieldList otherList)
		{
			int containedCount = 0;

			foreach (IField otherField in otherList.RecursiveEnumerator)
			{
				foreach (IField field in this.RecursiveEnumerator)
				{
					// if display names match...
					if (field.FieldString == otherField.FieldString)
					{
						// update field count
						containedCount++;
					}
				}
			}

			return (containedCount == otherList.Count);
		}


		/// <summary>
		/// Index from field name
		/// </summary>
		public int IndexOf(string name)
		{
			// for each field in list...
			for (int index = 0; index < Count; index++)
			{
				// if field name found in list
				if (this[index].FieldName == name)
				{
					// return list index
					return index;
				}
			}

			// field name not found
			return -1;
		}

		/// <summary>
		/// Indexer - get field by name
		/// </summary>
		public IField this[string name]
		{
			get
			{
				if (fieldsByName.ContainsKey(name))
				{
					return (IField)fieldsByName[name];
				}

				foreach (IField field in this)
				{
					if (field is FieldList || field is VariableList || field is FormItemList)
					{
						IField namedField = field[name];

						if (namedField != null)
						{
							return namedField;
						}
					}
				}

				//foreach (IField subField in this.RecursiveEnumerator)
				//{
				//    if (subField.FieldName == name)
				//    {
				//        return subField;
				//    }

				//    if (subField is IPaletteField)
				//    {
				//        if (((IPaletteField)subField).QualifiedFieldName == name)
				//        {
				//            return subField;
				//        }
				//    }
				//}

				return null;
			}
		}

		public static void DumpFields(IField fields, string messageText)
		{
			Console.WriteLine("FieldList.dumpfields: {0}", messageText);

			int i = 0;

			foreach (IField field in fields.RecursiveEnumerator)
			{
				Console.WriteLine(" field[{0}].fieldName = {1}", i, field.FieldName);
				i++;
			}
		}



		#region IField Interface

		private int fieldListId = 0;

		public string FieldName
		{
			get
			{
				return "Unnamed FieldList " + fieldListId;
			}
		}

		public string FieldString
		{
			get
			{
				return "<<" + FieldName + ">>";
			}
		}

		private int id = Project.NextUniqueID;

		public int Id
		{
			get { return id; }
		}

		#endregion

		#region IRecursiveEnumerable Interface

		public IEnumerable RecursiveEnumerator
		{
			get
			{
				foreach (IField item in Items)
				{
					foreach (IField field in item.RecursiveEnumerator)
					{
						yield return field;
					}
				}
			}
		}

		#endregion


		#region IAnyField Members


		public string QualifiedFieldName
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
