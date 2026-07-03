// $Workfile: ProjectFieldMapByName.cs $
// $Revision: 6 $	$Date: 8/23/07 8:49a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects
{
	// delegation rather than inheritance helps keep surface area of class low
	public class ProjectFieldMapByName 
	{
		public IField this[string name]
		{
			get 
			{
				return map.ContainsKey(name) ? map[name] : null;
			}
		}

		public void AddUnique(IPaletteField field)
		{
			if (!map.ContainsKey(field.FieldName))
			{
				map.Add(field.FieldName, field);
			}

			if (!map.ContainsKey(field.QualifiedFieldName))
			{
				map.Add(field.QualifiedFieldName, field);
			}
		}

		public void AddUnique(FormItem formItem)
		{
			add(formItem, formItem.AlternateLabel);
			add(formItem, formItem.QualifiedFieldName);
		}

		public void AddUnique(IMcqItem mcItem)
		{
			add(mcItem, mcItem.AlternateLabel);
			add(mcItem, mcItem.QualifiedFieldName);
		}

		private void add(IPaletteField field, string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				if (map.ContainsKey(name))
				{
					if (map[name] != field)
					{
						map.Remove(name);
						map.Add(name, field);
					}
				}
				else
				{
					map.Add(name, field);
				}
			}
		}

		public void AddUnique(IBlank blank)
		{
			add(blank, blank.AlternateLabel);
			add(blank, blank.QualifiedFieldName);
		}

		public int Count
		{
			get { return map.Count; }
		}

		public void Clear()
		{
			map.Clear();
		}

		public bool ContainsKey(string name)
		{
			return map.ContainsKey(name);
		}

		public void Remove(IPaletteField field)
		{
			map.Remove(field.FieldName);
			map.Remove(field.QualifiedFieldName);
		}

		private FieldMapByName map = new FieldMapByName();

		private class FieldMapByName : Dictionary<string, IField>
		{
		}
	}
}
