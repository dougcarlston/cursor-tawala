// $Workfile: FlatFieldList.cs $
// $Revision: 2 $	$Date: 5/24/06 7:20p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;

namespace Tawala.Projects
{
	/// <summary>
	/// List of fields.
	/// </summary>
	public class FlatFieldList : IListSource
	{
		private FieldList fieldList;

		public FlatFieldList(FieldList fieldList)
		{
			this.fieldList = fieldList;
		}

		public FieldList GetFlattenedList()
		{
			FieldList flatFieldList = new FieldList();

			if (fieldList != null)
			{
				foreach (IField field in fieldList.RecursiveEnumerator)
				{
					flatFieldList.Add(field);
				}
			}

			return (flatFieldList);
		}

		public int Count
		{
			get
			{
				return GetFlattenedList().Count;
			}
		}

		#region IListSource Interface

		public Boolean ContainsListCollection
		{
			get
			{
				return false;
			}
		}

		public IList GetList()
		{
			return GetFlattenedList();
		}

		#endregion

	}
}
