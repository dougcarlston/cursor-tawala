// $Workfile: ProjectFieldMapById.cs $
// $Revision: 6 $	$Date: 4/23/07 2:20p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Tawala.Projects
{
	// delegation rather than inheritance helps keep surface area of class low
	public sealed class ProjectFieldMapById 
	{
		public void AddUnique(IAnyField field)
		{
			map[field.Id] = field;
		}

		public IField FindField(string searchName)
		{
			return map.FindField(searchName);
		}

		public int Count
		{
			get { return map.Count; }
		}

		public IAnyField this[int id]
		{
			get { return id == -1 ? null : map[id]; }
		}

		public void Remove(int id)
		{
			map.Remove(id);
		}

		public bool ContainsKey(int id)
		{
			return map.ContainsKey(id);
		}

		public FieldIdMap.KeyCollection Keys
		{
			get { return map.Keys; }
		}

		public FieldIdMap.ValueCollection Values
		{
			get { return map.Values; }
		}

		public void Clear()
		{
			map.Clear();
		}

		public Dictionary<int, string> GetQualifiedFieldDictionary()
		{
			int capacity = Math.Max(16, map.Count / 2);

			Dictionary<int, string>  qualifiedMap = new Dictionary<int, string>(capacity);

			foreach (int id in map.Keys)
			{
				if (map[id] is IPaletteField)
				{
					qualifiedMap.Add(id, ((IPaletteField)map[id]).QualifiedFieldName);
				}
			}
			return qualifiedMap;
		}

		private FieldIdMap map = new FieldIdMap();

		private class FieldIdMap : Dictionary<int, IAnyField>
		{
			public IField FindField(string searchName)
			{
				Collection<IAnyField> fields = new Collection<IAnyField>();

				foreach (IAnyField field in this.Values)
				{
					fields.Add(field);
				}

				foreach (IField field in fields)
				{
					if (field is IPaletteField)
					{
						if (((IPaletteField)field).QualifiedFieldName.Equals(searchName))
						{
							return field;
						}
					}

					if (field.FieldName.Equals(searchName))
					{
						return field;
					}
				}

				return null;
			}
		}
	}
}
